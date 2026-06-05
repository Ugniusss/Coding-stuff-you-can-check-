using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance;

    public ChoiceGroup[] choiceGroups;
    public Button doneButton;
    public BotAI botAI; // Assign in OfflineBot scene only

    private List<PlayerAction>[] allPlayerActions = new List<PlayerAction>[2];

    private void Awake() => Instance = this;

    private void Start()
    {
        doneButton.onClick.AddListener(OnDonePressed);
        allPlayerActions[0] = new List<PlayerAction>();
        allPlayerActions[1] = new List<PlayerAction>();

        if (EventSystem.current != null)
            EventSystem.current.sendNavigationEvents = false;

        GameManager.Instance.currentPlayerIndex = 0;
        GameManager.Instance.ShowTurnImage();
        gameObject.SetActive(true);

        // Start first turn with ability phase
        StartTurnForCurrentPlayer();
    }

    void StartTurnForCurrentPlayer()
    {
        int index = GameManager.Instance.currentPlayerIndex;
        PlayerMover current  = GameManager.Instance.players[index];
        PlayerMover opponent = GameManager.Instance.players[1 - index];

        bool isBotTurn = botAI != null && index == 1;

        if (isBotTurn)
        {
            gameObject.SetActive(false);
            AbilityUI.Instance?.Hide();
            botAI.StartAbilityPhase(current, opponent, botAI.StartActionPhase);
        }
        else
        {
            // Show action menu and ability mini-panel together
            gameObject.SetActive(true);
            AbilityUI.Instance?.ShowForPlayer(current, opponent);
        }
    }

    void OnDonePressed()
    {
        int currentIndex = GameManager.Instance.currentPlayerIndex;
        List<PlayerAction> currentPlayerActions = new();

        for (int i = 0; i < choiceGroups.Length; i++)
        {
            string choice = choiceGroups[i].GetSelectedName();
            if (string.IsNullOrEmpty(choice)) return;

            if (choice == "YesB") { currentPlayerActions.Add(new PlayerAction(null, true));  continue; }
            if (choice == "NoB")  { currentPlayerActions.Add(new PlayerAction(null, false)); continue; }

            string colorString = ConvertButtonToColor(choice);
            if (colorString != null && System.Enum.TryParse(colorString, out PlayerAction.PathColor color))
                currentPlayerActions.Add(new PlayerAction(color, false));
            else
                return;
        }

        AbilityUI.Instance?.Hide();
        allPlayerActions[currentIndex] = currentPlayerActions;
        StartCoroutine(ExecutePlannedActions(currentPlayerActions.ToArray()));
    }

    // Used by BotAI to submit actions directly, bypassing UI
    public void SubmitActionsDirectly(PlayerAction[] actions)
    {
        StartCoroutine(ExecutePlannedActions(actions));
    }

    public IEnumerator ExecutePlannedActions(PlayerAction[] actions)
    {
        int index = GameManager.Instance.currentPlayerIndex;
        if (GameManager.Instance?.players == null || index >= GameManager.Instance.players.Length) yield break;

        PlayerMover mover = GameManager.Instance.players[index];
        if (mover == null) yield break;

        // Check if ambushed — skip all move actions
        AbilityManager abilityMgr = mover.GetComponent<AbilityManager>();
        bool skipMoves = abilityMgr != null && abilityMgr.ambushed;

        string current = mover.currentCityName;

        foreach (var action in actions)
        {
            if (action.moveColor != null && !skipMoves)
            {
                string nextCity = mover.FindNextCityByColor(current, action.moveColor.ToString());
                if (!string.IsNullOrEmpty(nextCity))
                {
                    Transform nextMarker = mover.FindCityMarker(nextCity);
                    if (nextMarker != null)
                    {
                        yield return mover.SlideTo(nextMarker.position);
                        current = nextCity;
                        mover.currentCityName = current;
                    }
                }
            }

            mover.CheckForChestAtCurrentCity(action.doAction);
            if (action.doAction)
            {
                yield return new WaitUntil(() => !MarketUI.IsOpen && !GamblerUI.IsOpen);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);
            }
        }

        BotAI.IsBotTurn = false;

        foreach (var group in choiceGroups)
            group.ResetSelection();

        GameManager.Instance.currentPlayerIndex++;

        if (GameManager.Instance.currentPlayerIndex > 1)
        {
            GameManager.Instance.currentPlayerIndex = 0;
            GameManager.Instance.AdvanceTurn();
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.turnChange);
        GameManager.Instance.ShowTurnImage();

        // Start ability phase for next player
        StartTurnForCurrentPlayer();
    }

    string ConvertButtonToColor(string buttonName) => buttonName switch
    {
        "RedB"    => "Red",
        "BlueB"   => "Blue",
        "YellowB" => "Yellow",
        _ => null
    };
}
