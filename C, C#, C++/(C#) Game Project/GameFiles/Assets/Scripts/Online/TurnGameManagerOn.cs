// TODO: Bandit den — should steal -1 of ALL items, not just 1 random
// TODO: Check ambush — verify effect is actually applied on opponent's turn
// TODO: Legend button — still floating in Online scene (path: actionMenuPanel/InfoPanelCanvas/InfoSystem/InfoButton)
// TODO: Ready UI — add a ready/waiting screen before game starts
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnGameManagerOn : MonoBehaviourPun
{
    public static TurnGameManagerOn Instance;

    public int currentTurnActor;
    public int turnCounter;
    public int maxTurns = 15;

    int actionsThisTurn = 0;
    bool _waitingForBuilding;
    readonly Dictionary<int, int> _gamblerBanUntilTurn = new();

    void Awake() { Instance = this; }

    // Game started by ClassSelectionManagerOn.RPC_StartGame -> MasterStartGame()
    // Fallback: if no class selection canvas in scene, start directly
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (FindObjectOfType<ClassSelectionManagerOn>() == null)
            MasterStartGame();
    }

    public void MasterStartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        actionsThisTurn = 0;
        turnCounter = 1;
        StartCoroutine(StartGameDelayed());
    }

    IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC(nameof(RPC_SetTurnCounter), RpcTarget.All, 1);
        int firstActor = PhotonNetwork.PlayerList[0].ActorNumber;
        photonView.RPC(nameof(RPC_SetTurn), RpcTarget.All, firstActor);
    }

    // ===== CALLED FROM UI =====
    public void SubmitChoices(string[] choices)
    {
        photonView.RPC(nameof(RPC_SubmitChoices), RpcTarget.MasterClient,
            PhotonNetwork.LocalPlayer.ActorNumber, choices);
    }

    // ===== MASTER ONLY =====
    [PunRPC]
    void RPC_SubmitChoices(int actor, string[] choices)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (actor != currentTurnActor) return;
        StartCoroutine(ExecuteChoices(actor, choices));
    }

    [PunRPC]
    void RPC_SetTurnCounter(int value)
    {
        turnCounter = value;
        UIPlius.Instance?.SetTurnCounter(value);
    }

    IEnumerator ExecuteChoices(int actor, string[] choices)
    {
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actor, out var mover)) yield break;

        // --- ABILITY PHASE ---
        bool hasAbility = choices.Length == 5;
        string abilityStr = hasAbility ? choices[0] : "None";
        int moveStart = hasAbility ? 1 : 0;

        if (!string.IsNullOrEmpty(abilityStr) && abilityStr != "None")
            yield return StartCoroutine(ApplyAbility(actor, abilityStr, mover));

        // --- MOVE PHASE ---
        // Refresh from mover after ability (teleport changes the city)
        string current = abilityStr.StartsWith("Teleport:")
            ? abilityStr.Substring("Teleport:".Length)
            : mover.currentCityName;
        for (int ci = moveStart; ci < choices.Length; ci++)
        {
            NetActionType action = Parse(choices[ci]);

            if (action == NetActionType.Interact)
            {
                mover.Master_Interact(current);
                yield return StartCoroutine(CheckBuildingAt(actor, current, mover));
                continue;
            }

            string color = action switch
            {
                NetActionType.MoveRed    => "Red",
                NetActionType.MoveBlue   => "Blue",
                NetActionType.MoveYellow => "Yellow",
                _                        => null
            };
            if (color == null) continue;

            string nextCity = mover.FindNextCityByColor(current, color);
            if (string.IsNullOrEmpty(nextCity)) continue;

            Transform marker = mover.FindCityMarker(nextCity);
            if (marker == null) continue;

            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SlideTo), RpcTarget.All, marker.position);
            yield return new WaitForSeconds(mover.EstimatedSlideSeconds());
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SetCity), RpcTarget.All, nextCity);
            current = nextCity;

            yield return StartCoroutine(CheckBanditDenAt(actor, current, mover));
        }

        // TURN END
        actionsThisTurn++;
        MobuObjectSpawner.Instance?.CheckShrineRespawns(turnCounter);

        if (actionsThisTurn >= 2)
        {
            actionsThisTurn = 0;
            turnCounter++;
            photonView.RPC(nameof(RPC_SetTurnCounter), RpcTarget.All, turnCounter);

            if (turnCounter > maxTurns)
            {
                photonView.RPC(nameof(RPC_ShowWin), RpcTarget.All, CalculateWinner());
                yield break;
            }
        }

        int nextActor = PhotonNetwork.PlayerList[0].ActorNumber == actor
            ? PhotonNetwork.PlayerList[1].ActorNumber
            : PhotonNetwork.PlayerList[0].ActorNumber;
        photonView.RPC(nameof(RPC_SetTurn), RpcTarget.All, nextActor);
    }

    IEnumerator ApplyAbility(int actor, string abilityStr, PlayerMoverOn mover)
    {
        int opponentActor = PhotonNetwork.PlayerList[0].ActorNumber == actor
            ? PhotonNetwork.PlayerList[1].ActorNumber
            : PhotonNetwork.PlayerList[0].ActorNumber;

        PlayerMoverOn.MoversByActor.TryGetValue(opponentActor, out var oppMover);

        if (abilityStr == "Bribe" && oppMover != null)
        {
            var oppMgr = oppMover.GetComponent<AbilityManager>();
            if (oppMgr == null || !oppMgr.fortified)
            {
                var selfInv = mover.GetComponent<InventoryOn>();
                var oppInv  = oppMover.GetComponent<InventoryOn>();
                int oppGold = oppInv.GetItemCount(InventoryOn.ItemType.Gold);

                if (oppGold == 0)
                {
                    var available = new List<InventoryOn.ItemType>();
                    foreach (InventoryOn.ItemType t in System.Enum.GetValues(typeof(InventoryOn.ItemType)))
                        if (t != InventoryOn.ItemType.Gold && oppInv.GetItemCount(t) > 0) available.Add(t);

                    if (available.Count > 0)
                    {
                        var stolen = available[Random.Range(0, available.Count)];
                        oppMover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)stolen, -1);
                        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)stolen, 1);
                        photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} bribed — stole 1 {stolen}!");
                    }
                    else
                        photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} tried Bribe — opponent has nothing!");
                }
                else
                {
                    int stolen = Mathf.Max(1, Mathf.FloorToInt(oppGold * Random.Range(0.3f, 0.5f)));
                    oppMover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Gold, -stolen);
                    mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Gold, stolen);
                    photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} bribed — stole {stolen}g!");
                }
            }
            else
                photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} tried Bribe — blocked by Fortify!");

            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ConsumeAbilityCharge), RpcTarget.All, (int)AbilityType.Bribe);
        }
        else if (abilityStr == "Ambush" && oppMover != null)
        {
            var oppMgr = oppMover.GetComponent<AbilityManager>();
            if (oppMgr == null || !oppMgr.fortified)
            {
                oppMover.photonView.RPC(nameof(PlayerMoverOn.RPC_SetAmbushed), RpcTarget.All, true);
                photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} ambushed Player {opponentActor}!");
            }
            else
                photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} tried Ambush — blocked by Fortify!");

            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ConsumeAbilityCharge), RpcTarget.All, (int)AbilityType.Ambush);
        }
        else if (abilityStr == "Fortify")
        {
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SetFortify), RpcTarget.All, 3);
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ConsumeAbilityCharge), RpcTarget.All, (int)AbilityType.Fortify);
            photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} fortified for 3 turns!");
        }
        else if (abilityStr.StartsWith("Teleport:"))
        {
            string targetCity = abilityStr.Substring("Teleport:".Length);
            Transform marker = mover.FindCityMarker(targetCity);
            if (marker != null)
            {
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SlideTo), RpcTarget.All, marker.position);
                yield return new WaitForSeconds(mover.EstimatedSlideSeconds());
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SetCity), RpcTarget.All, targetCity);
                photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} teleported to {targetCity}!");
            }
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ConsumeAbilityCharge), RpcTarget.All, (int)AbilityType.Teleport);
        }
    }

    IEnumerator CheckBanditDenAt(int actor, string cityName, PlayerMoverOn mover)
    {
        if (!MobuObjectSpawner.banditCities.Contains(cityName)) yield break;

        if (ClassData.BanditDenImmune(mover.playerClass))
        {
            photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} (Wanderer) immune to Bandit Den!");
            yield break;
        }

        var inv = mover.GetComponent<InventoryOn>();
        var available = new List<InventoryOn.ItemType>();
        foreach (InventoryOn.ItemType t in System.Enum.GetValues(typeof(InventoryOn.ItemType)))
            if (inv.GetItemCount(t) > 0) available.Add(t);

        if (available.Count == 0)
            photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} hit Bandit Den — nothing to steal!");
        else
        {
            var stolen = available[Random.Range(0, available.Count)];
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)stolen, -1);
            photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} lost 1 {stolen} to Bandit Den!");
        }
    }

    IEnumerator CheckBuildingAt(int actor, string cityName, PlayerMoverOn mover)
    {
        if (MobuObjectSpawner.shrineCities.Contains(cityName))
        {
            var values = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
            AbilityType granted = values[Random.Range(0, values.Length)];
            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_GrantAbilityCharge), RpcTarget.All, (int)granted);
            photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} visited Shrine — got {granted} charge!");

            MobuObjectSpawner.Instance.photonView.RPC(
                nameof(MobuObjectSpawner.RPC_DestroyBuilding), RpcTarget.All, cityName);
            MobuObjectSpawner.Instance.ScheduleShrineRespawn(turnCounter + 5);
            yield break;
        }

        if (MobuObjectSpawner.marketCities.Contains(cityName))
        {
            if (MarketUIOn.Instance == null) { photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} at Market (UI missing)"); yield break; }
            _waitingForBuilding = true;
            if (actor == PhotonNetwork.LocalPlayer.ActorNumber)
                MarketUIOn.Instance.OpenMarket(mover.GetComponent<InventoryOn>(), actor);
            else
                photonView.RPC(nameof(RPC_OpenMarketOn), RpcTarget.All, actor);

            while (_waitingForBuilding) yield return null;
            yield break;
        }

        if (MobuObjectSpawner.gamblerCities.Contains(cityName))
        {
            if (GamblerUIOn.Instance == null) { photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All, $"Player {actor} at Gambler (UI missing)"); yield break; }

            if (_gamblerBanUntilTurn.TryGetValue(actor, out int banUntil) && turnCounter <= banUntil)
            {
                int left = banUntil - turnCounter + 1;
                photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All,
                    $"Player {actor} cannot gamble for {left} more turn(s)!");
                yield break;
            }

            _waitingForBuilding = true;
            if (actor == PhotonNetwork.LocalPlayer.ActorNumber)
                GamblerUIOn.Instance.OpenGambler(mover.GetComponent<InventoryOn>(), actor);
            else
                photonView.RPC(nameof(RPC_OpenGamblerOn), RpcTarget.All, actor);

            while (_waitingForBuilding) yield return null;
        }
    }

    [PunRPC]
    void RPC_OpenMarketOn(int actorNum)
    {
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actorNum, out var mover)) return;
        if (!mover.photonView.IsMine) return;
        MarketUIOn.Instance?.OpenMarket(mover.GetComponent<InventoryOn>(), actorNum);
    }

    [PunRPC]
    void RPC_OpenGamblerOn(int actorNum)
    {
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actorNum, out var mover)) return;
        if (!mover.photonView.IsMine) return;
        GamblerUIOn.Instance?.OpenGambler(mover.GetComponent<InventoryOn>(), actorNum);
    }

    [PunRPC]
    public void RPC_BuildingClosed(int actorNum)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _waitingForBuilding = false;
    }

    [PunRPC]
    public void RPC_MarketBuyItem(int actorNum, int itemType, int cost)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actorNum, out var mover)) return;
        var inv = mover.GetComponent<InventoryOn>();
        if (inv == null || inv.GetItemCount(InventoryOn.ItemType.Gold) < cost) return;

        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Gold, -cost);
        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, itemType, 1);
        photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All,
            $"Player {actorNum} bought 1 {(InventoryOn.ItemType)itemType} from Market ({cost}g)");
    }

    [PunRPC]
    public void RPC_MarketBuyAbility(int actorNum, int abilityType, int cost)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actorNum, out var mover)) return;
        var inv = mover.GetComponent<InventoryOn>();
        if (inv == null || inv.GetItemCount(InventoryOn.ItemType.Gold) < cost) return;

        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Gold, -cost);
        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_GrantAbilityCharge), RpcTarget.All, abilityType);
        photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All,
            $"Player {actorNum} bought {(AbilityType)abilityType} scroll ({cost}g)");
    }

    [PunRPC]
    public void RPC_GamblerBet(int actorNum, bool choseOdd)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(actorNum, out var mover)) return;
        var inv = mover.GetComponent<InventoryOn>();
        if (inv == null || inv.GetItemCount(InventoryOn.ItemType.Gold) < 5) return;

        int roll = Random.Range(1, 7);
        bool won = choseOdd == (roll % 2 != 0);
        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Gold, won ? 5 : -5);
        _gamblerBanUntilTurn[actorNum] = turnCounter + 5;
        photonView.RPC(nameof(RPC_GamblerResult), RpcTarget.All, actorNum, roll, won);
        photonView.RPC(nameof(RPC_AddGameLog), RpcTarget.All,
            $"Player {actorNum} gambled: rolled {roll}, {(won ? "won +5g" : "lost -5g")}!");
    }

    [PunRPC]
    public void RPC_GamblerResult(int actorNum, int roll, bool won)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
            GamblerUIOn.Instance?.ShowResult(roll, won);
    }

    [PunRPC]
    public void RPC_AddGameLog(string message)
    {
        GameLog.Instance?.Add(message);
    }

    [PunRPC]
    void RPC_SetTurn(int actor)
    {
        currentTurnActor = actor;
        UIPlius.Instance?.SetTurn(actor);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.turnChange);

        int local = PhotonNetwork.LocalPlayer.ActorNumber;
        if (actor == local) AbilityUIOn.Instance?.ShowForTurn();
        else                AbilityUIOn.Instance?.Hide();
    }

    [PunRPC]
    void RPC_ShowWin(int winnerActor)
    {
        UIPlius.Instance?.ShowWin(winnerActor);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.win);
    }

    int CalculateWinner()
    {
        InventoryOn p1 = PlayerMoverOn.MoversByActor[1].GetComponent<InventoryOn>();
        InventoryOn p2 = PlayerMoverOn.MoversByActor[2].GetComponent<InventoryOn>();
        int gold1 = p1.GetItemCount(InventoryOn.ItemType.Gold);
        int gold2 = p2.GetItemCount(InventoryOn.ItemType.Gold);
        if (gold1 > gold2) return 1;
        if (gold2 > gold1) return 2;
        int items1 = p1.GetTotalItems();
        int items2 = p2.GetTotalItems();
        if (items1 > items2) return 1;
        if (items2 > items1) return 2;
        return 1;
    }

    enum NetActionType { None, Interact, MoveRed, MoveBlue, MoveYellow }

    NetActionType Parse(string choice) => choice switch
    {
        "YesB"    => NetActionType.Interact,
        "RedB"    => NetActionType.MoveRed,
        "BlueB"   => NetActionType.MoveBlue,
        "YellowB" => NetActionType.MoveYellow,
        _         => NetActionType.None
    };
}
