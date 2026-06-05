using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;



public class PlayerMoverOn : MonoBehaviourPun
{
    public static Dictionary<int, PlayerMoverOn> MoversByActor = new();
    public int ownerActor;



    public enum PathColor { Red, Blue, Yellow }

    [Header("State")]
    public string currentCityName;
    [HideInInspector] public string previousCity;
    public PlayerClass playerClass = PlayerClass.Merchant;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("World refs (auto)")]
    public Transform citiesParent;

    void Start()
    {
        citiesParent = WorldRefs.Instance.cities;

        ownerActor = photonView.OwnerActorNr;
        MoversByActor[ownerActor] = this;
        Debug.Log($"[PlayerMoverOn] Registered actor {ownerActor}");
    }

    void OnDestroy()
    {
        if (MoversByActor.TryGetValue(ownerActor, out var current) && current == this)
            MoversByActor.Remove(ownerActor);
    }

    // ---------- MOVEMENT ----------

    [PunRPC]
    public void RPC_SlideTo(Vector3 target)
    {
        StopAllCoroutines();
        StartCoroutine(Slide(target));
    }

    IEnumerator Slide(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }

    public float EstimatedSlideSeconds() => 1f / moveSpeed;

    [PunRPC]
    public void RPC_SetCity(string city)
    {
        currentCityName = city;
    }

    // ---------- PATH ----------

    public string FindNextCityByColor(string from, string color)
    {
        List<string> connections = PathFinder.Instance.GetConnections(from, color);
        if (connections == null || connections.Count == 0) return null;

        foreach (var c in connections)
            if (c != previousCity)
                return c;

        return connections[0];
    }

    public Transform FindCityMarker(string cityName)
    {
        Transform city = citiesParent.Find("City_" + cityName);
        return city ? city.Find("PlayerPositions/VisualMarker") : null;
    }

    // ---------- INTERACTION (MASTER ONLY) ----------

    public void Master_Interact(string city)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (MobuObjectSpawner.chestCities.Contains(city))
        {
            photonView.RPC(nameof(RPC_PlayChestSound), RpcTarget.All);
            ApplyChestReward();
            MobuObjectSpawner.chestCities.Remove(city);
            MobuObjectSpawner.Instance.SpawnChestAtFreeCity();
            DestroyMob(city, "Chest");
        }
        else if (MobuObjectSpawner.towerCities.Contains(city))
        {
            photonView.RPC(nameof(RPC_PlayTowerSound), RpcTarget.All);
            InventoryOn inv = GetComponent<InventoryOn>();
            if (inv == null) return;

            int threshold = ClassData.TowerThreshold(playerClass);
            if (inv.GetItemCount(InventoryOn.ItemType.Sword) >= threshold &&
                inv.GetItemCount(InventoryOn.ItemType.Shield) >= threshold &&
                inv.GetItemCount(InventoryOn.ItemType.Bow) >= threshold)
            {
                ModifyAll(InventoryOn.ItemType.Sword, -threshold);
                ModifyAll(InventoryOn.ItemType.Shield, -threshold);
                ModifyAll(InventoryOn.ItemType.Bow, -threshold);
                ModifyAll(InventoryOn.ItemType.Gold, 10);

                MobuObjectSpawner.towerCities.Remove(city);
                DestroyMob(city, "Tower");
                TurnGameManagerOn.Instance?.photonView.RPC(
                    nameof(TurnGameManagerOn.RPC_AddGameLog), RpcTarget.All,
                    $"Player {ownerActor} cleared a tower! Paid {threshold} weapons, +10g!");
            }
            else
            {
                ModifyAll(InventoryOn.ItemType.Gold, -5);
                TurnGameManagerOn.Instance?.photonView.RPC(
                    nameof(TurnGameManagerOn.RPC_AddGameLog), RpcTarget.All,
                    $"Player {ownerActor} tried tower — needs {threshold} of each weapon! -5g!");
            }
        }
    }

    void ApplyChestReward()
    {
        InventoryOn.ItemType item = (InventoryOn.ItemType)Random.Range(0, 4);
        int[] amounts = { 1, 2, 3 };
        int amount = amounts[Random.Range(0, amounts.Length)];
        ModifyAll(item, amount);
        string sign = amount >= 0 ? "+" : "";
        TurnGameManagerOn.Instance?.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_AddGameLog), RpcTarget.All,
            $"Player {ownerActor} opened chest — {item} {sign}{amount}");
    }

    void ModifyAll(InventoryOn.ItemType type, int amount)
    {
        photonView.RPC(nameof(RPC_ModifyItem), RpcTarget.All, (int)type, amount);
    }

    [PunRPC]
    public void RPC_ModifyItem(int type, int amount)
    {
        GetComponent<InventoryOn>()?.ModifyItem((InventoryOn.ItemType)type, amount);
    }

    [PunRPC]
    public void RPC_SetPlayerClass(int classIndex)
    {
        playerClass = (PlayerClass)classIndex;
    }

    [PunRPC]
    public void RPC_GrantAbilityCharge(int abilityType)
    {
        GetComponent<AbilityManager>()?.GrantFreeAbilityCharge((AbilityType)abilityType);
        if (photonView.IsMine) AbilityUIOn.Instance?.RefreshUI();
    }

    [PunRPC]
    public void RPC_SetAmbushed(bool value)
    {
        var mgr = GetComponent<AbilityManager>();
        if (mgr != null) { var f = typeof(AbilityManager).GetField("ambushed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); if (f != null) f.SetValue(mgr, value); }
    }

    [PunRPC]
    public void RPC_SetFortify(int turns)
    {
        var mgr = GetComponent<AbilityManager>();
        if (mgr != null) { var f = typeof(AbilityManager).GetField("fortifyTurnsLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); if (f != null) f.SetValue(mgr, turns); }
    }

    [PunRPC]
    public void RPC_ConsumeAbilityCharge(int abilityType)
    {
        GetComponent<AbilityManager>()?.ConsumeCharge((AbilityType)abilityType);
        if (photonView.IsMine) AbilityUIOn.Instance?.RefreshUI();
    }

    void DestroyMob(string city, string tag)
    {
        Transform c = citiesParent.Find("City_" + city);
        if (!c) return;

        foreach (Transform t in c.Find("MobPositions"))
        {
            if (t.CompareTag(tag))
            {
                PhotonView pv = t.GetComponent<PhotonView>();
                if (pv) PhotonNetwork.Destroy(pv);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_PlayChestSound()
    {
        AudioManager.Instance?.PlaySFX(
            AudioManager.Instance.chest
        );
    }

    [PunRPC]
    void RPC_PlayTowerSound()
    {
        AudioManager.Instance?.PlaySFX(
            AudioManager.Instance.tower
        );
    }

}
