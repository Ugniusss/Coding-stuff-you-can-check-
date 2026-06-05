using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MobuObjectSpawner : MonoBehaviourPunCallbacks
{
    public Transform citiesParent;
    public GameObject towerPrefab;
    public GameObject chestPrefab;

    [Header("Buildings")]
    public GameObject marketPrefab;
    public GameObject banditDenPrefab;
    public GameObject shrinePrefab;
    public GameObject gamblerPrefab;

    public static MobuObjectSpawner Instance;

    public int towerCount  = 5;
    public int chestCount  = 10;

    public static HashSet<string> towerCities   = new();
    public static HashSet<string> chestCities   = new();
    public static HashSet<string> marketCities  = new();
    public static HashSet<string> banditCities  = new();
    public static HashSet<string> shrineCities  = new();
    public static HashSet<string> gamblerCities = new();
    public static HashSet<string> buildingCities = new(); // all 4 types combined

    private List<Transform> availableMarkers = new();
    private readonly HashSet<string> usedCities = new();
    private readonly List<int> pendingShrineRespawns = new();

    private bool spawned;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => TrySpawn();

    public override void OnJoinedRoom() => TrySpawn();

    void TrySpawn()
    {
        if (spawned) return;
        if (!PhotonNetwork.InRoom) return;
        if (!PhotonNetwork.IsMasterClient) return;

        CollectAllMarkers();
        SpawnTowers();
        SpawnBuildings();
        SpawnChests();
        spawned = true;
    }

    void CollectAllMarkers()
    {
        availableMarkers.Clear();
        foreach (Transform city in citiesParent)
        {
            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;
            Transform marker = mobPos.Find("VisualMarker");
            if (marker != null) availableMarkers.Add(marker);
        }
    }

    void SpawnTowers()
    {
        usedCities.Clear();
        for (int i = 0; i < towerCount && availableMarkers.Count > 0;)
        {
            int idx = Random.Range(0, availableMarkers.Count);
            Transform marker = availableMarkers[idx];
            availableMarkers.RemoveAt(idx);
            string city = marker.parent.parent.name.Replace("City_", "");
            if (usedCities.Contains(city) || chestCities.Contains(city)) continue;

            GameObject t = PhotonNetwork.Instantiate(towerPrefab.name, marker.position, Quaternion.Euler(0, 90, 0));
            t.transform.SetParent(marker.parent);
            towerCities.Add(city); usedCities.Add(city); i++;
        }
    }

    void SpawnBuildings()
    {
        var pool = new List<(GameObject prefab, int type, Vector3 rot)>();
        if (marketPrefab   != null) pool.Add((marketPrefab,    0, new Vector3(0, 90, 0)));
        if (banditDenPrefab!= null) pool.Add((banditDenPrefab, 1, new Vector3(0, 90, 0)));
        if (shrinePrefab   != null) pool.Add((shrinePrefab,    2, new Vector3(0, 90, 0)));
        if (gamblerPrefab  != null) pool.Add((gamblerPrefab,   3, new Vector3(0, 90, 0)));

        foreach (var (prefab, btype, rot) in pool)
        {
            for (int attempt = 0; attempt < availableMarkers.Count + 10; attempt++)
            {
                if (availableMarkers.Count == 0) break;
                int idx = Random.Range(0, availableMarkers.Count);
                Transform marker = availableMarkers[idx];
                availableMarkers.RemoveAt(idx);
                string city = marker.parent.parent.name.Replace("City_", "");
                if (usedCities.Contains(city) || towerCities.Contains(city)) continue;

                photonView.RPC(nameof(RPC_SpawnBuilding), RpcTarget.All,
                    city, btype, marker.position);
                usedCities.Add(city);
                break;
            }
        }
    }

    void SpawnChests()
    {
        usedCities.Clear();
        for (int i = 0; i < chestCount && availableMarkers.Count > 0;)
        {
            int idx = Random.Range(0, availableMarkers.Count);
            Transform marker = availableMarkers[idx];
            availableMarkers.RemoveAt(idx);
            string city = marker.parent.parent.name.Replace("City_", "");
            if (usedCities.Contains(city) || towerCities.Contains(city) || buildingCities.Contains(city)) continue;

            GameObject c = PhotonNetwork.Instantiate(chestPrefab.name, marker.position, Quaternion.Euler(0, 270, 0));
            c.transform.SetParent(marker.parent);
            chestCities.Add(city); usedCities.Add(city); i++;
        }
    }

    [PunRPC]
    void RPC_SpawnBuilding(string cityName, int buildingType, Vector3 pos)
    {
        GameObject prefab = buildingType switch
        {
            0 => marketPrefab,
            1 => banditDenPrefab,
            2 => shrinePrefab,
            3 => gamblerPrefab,
            _ => null
        };
        if (prefab == null) return;

        Transform city = citiesParent.Find("City_" + cityName);
        if (city == null) return;
        Transform mobPos = city.Find("MobPositions");

        GameObject go = Instantiate(prefab, pos, Quaternion.Euler(0, 90, 0));
        if (mobPos != null) go.transform.SetParent(mobPos, true);
        go.transform.localScale = buildingType switch
        {
            0 => Vector3.one * 0.75f,  // Market — larger
            3 => Vector3.one * 0.25f,  // Gambler
            _ => Vector3.one * 0.5f
        };

        switch (buildingType)
        {
            case 0: marketCities.Add(cityName);  break;
            case 1: banditCities.Add(cityName);  break;
            case 2: shrineCities.Add(cityName);  break;
            case 3: gamblerCities.Add(cityName); break;
        }
        buildingCities.Add(cityName);
    }

    [PunRPC]
    public void RPC_DestroyBuilding(string cityName)
    {
        Transform city = citiesParent.Find("City_" + cityName);
        if (city != null)
        {
            Transform mobPos = city.Find("MobPositions");
            if (mobPos != null)
            {
                foreach (Transform child in mobPos)
                {
                    string n = child.name;
                    if (n.StartsWith("Market") || n.StartsWith("BanditDen") ||
                        n.StartsWith("Shrine") || n.StartsWith("Gambler"))
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }
        marketCities.Remove(cityName);
        banditCities.Remove(cityName);
        shrineCities.Remove(cityName);
        gamblerCities.Remove(cityName);
        buildingCities.Remove(cityName);
    }

    public void SpawnChestAtFreeCity()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var freeMarkers = new List<Transform>();
        foreach (Transform city in citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (chestCities.Contains(cityName) || towerCities.Contains(cityName) || buildingCities.Contains(cityName)) continue;
            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;
            Transform marker = mobPos.Find("VisualMarker");
            if (marker != null) freeMarkers.Add(marker);
        }
        if (freeMarkers.Count == 0) return;
        Transform chosen = freeMarkers[Random.Range(0, freeMarkers.Count)];
        string chosenCity = chosen.parent.parent.name.Replace("City_", "");
        GameObject chest = PhotonNetwork.Instantiate(chestPrefab.name, chosen.position, Quaternion.Euler(0, 270, 0));
        chest.transform.SetParent(chosen.parent);
        chestCities.Add(chosenCity);
    }

    public void ScheduleShrineRespawn(int targetTurn)
    {
        if (PhotonNetwork.IsMasterClient)
            pendingShrineRespawns.Add(targetTurn);
    }

    public void CheckShrineRespawns(int currentTurn)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        for (int i = pendingShrineRespawns.Count - 1; i >= 0; i--)
        {
            if (currentTurn >= pendingShrineRespawns[i])
            {
                pendingShrineRespawns.RemoveAt(i);
                SpawnShrineAtFreeCity();
            }
        }
    }

    void SpawnShrineAtFreeCity()
    {
        if (shrinePrefab == null) return;
        var freeMarkers = new List<Transform>();
        foreach (Transform city in citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (towerCities.Contains(cityName) || chestCities.Contains(cityName) || buildingCities.Contains(cityName)) continue;
            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;
            Transform marker = mobPos.Find("VisualMarker");
            if (marker != null) freeMarkers.Add(marker);
        }
        if (freeMarkers.Count == 0) return;
        Transform chosen = freeMarkers[Random.Range(0, freeMarkers.Count)];
        string chosenCity = chosen.parent.parent.name.Replace("City_", "");
        photonView.RPC(nameof(RPC_SpawnBuilding), RpcTarget.All, chosenCity, 2, chosen.position);

        photonView.RPC(nameof(TurnGameManagerOn.RPC_AddGameLog), RpcTarget.All, $"Shrine appeared at {chosenCity}!");
    }
}
