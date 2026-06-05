using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MobObjectSpawner : MonoBehaviour
{
    public Transform citiesParent;
    public GameObject towerPrefab;
    public GameObject chestPrefab;

    [Header("Buildings")]
    public GameObject marketPrefab;
    public GameObject banditDenPrefab;
    public GameObject shrinePrefab;
    public GameObject wanderingGamblerPrefab;
    public int buildingCount = 2;

    public static MobObjectSpawner Instance;
    public static HashSet<string> towerCities = new();
    public static HashSet<string> chestCities = new();
    public static HashSet<string> buildingCities = new();

    public int towerCount = 5;
    public int chestCount = 10;

    private List<Transform> availableMarkers = new List<Transform>();
    private List<int> pendingShrineRespawns = new();

    void Start()
    {
        Instance = this;
        towerCities = new();
        chestCities = new();
        buildingCities = new();

        CollectAllMarkers();
        SpawnTowers();
        SpawnBuildings();
        SpawnChests();
    }
    public void SpawnChestAtFreeCity()
    {
        List<Transform> freeMarkers = new();

        foreach (Transform city in citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (chestCities.Contains(cityName)) continue;

            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;

            
            bool hasTower = false;
            foreach (Transform child in mobPos)
            {
                if (child.CompareTag("Tower"))
                {
                    hasTower = true;
                    break;
                }
            }

            if (hasTower) continue;
            if (buildingCities.Contains(cityName)) continue;

            Transform marker = mobPos.Find("VisualMarker");
            if (marker != null)
            {
                freeMarkers.Add(marker);
            }
        }

        if (freeMarkers.Count == 0)
        {
            //Debug.Log("No free city found to respawn chest.");
            return;
        }

        Transform chosen = freeMarkers[Random.Range(0, freeMarkers.Count)];
        string chosenCityName = chosen.parent.parent.name.Replace("City_", "");

        GameObject chest = Instantiate(chestPrefab, chosen.position, Quaternion.Euler(0, 270, 0));
        chest.transform.SetParent(chosen.parent); 
        chest.tag = "Chest";

        chestCities.Add(chosenCityName);
        //Debug.Log($"Chest respawned in city {chosenCityName}");
    }


    public void ScheduleShrineRespawn(int targetTurn)
    {
        pendingShrineRespawns.Add(targetTurn);
    }

    public void CheckShrineRespawns()
    {
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0;
        for (int i = pendingShrineRespawns.Count - 1; i >= 0; i--)
        {
            if (currentTurn >= pendingShrineRespawns[i])
            {
                pendingShrineRespawns.RemoveAt(i);
                SpawnShrineAtFreeCity();
            }
        }
    }

    public void SpawnShrineAtFreeCity()
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

        GameObject building = Instantiate(shrinePrefab, chosen.position, Quaternion.Euler(0, 90, 0));
        building.transform.SetParent(chosen.parent);
        building.tag = "Building";

        BuildingInteraction bi = building.GetComponent<BuildingInteraction>();
        if (bi == null) bi = building.AddComponent<BuildingInteraction>();
        bi.buildingType = BuildingType.Shrine;

        buildingCities.Add(chosenCity);
        GameLog.Instance?.Add($"Shrine appeared at {chosenCity}!");
    }

    void CollectAllMarkers()
    {
        availableMarkers.Clear();

        foreach (Transform city in citiesParent)
        {
            Transform mobPositions = city.Find("MobPositions");
            if (mobPositions == null) continue;

            Transform marker = mobPositions.Find("VisualMarker");
            if (marker != null)
            {
                availableMarkers.Add(marker);
            }
        }
    }

    HashSet<string> usedCities = new();

    void SpawnTowers()
    {
        usedCities.Clear();

        for (int i = 0; i < towerCount && availableMarkers.Count > 0;)
        {
            int index = Random.Range(0, availableMarkers.Count);
            Transform marker = availableMarkers[index];
            availableMarkers.RemoveAt(index);

            string cityName = marker.parent.parent.name.Replace("City_", "");

            if (usedCities.Contains(cityName) || chestCities.Contains(cityName))
                continue;

            GameObject tower = Instantiate(towerPrefab, marker.position, Quaternion.Euler(0, 90, 0));
            tower.transform.SetParent(marker.parent);

            towerCities.Add(cityName);
            usedCities.Add(cityName);
            i++;
        }
    }





    void SpawnBuildings()
    {
        // Collect available building prefabs
        var buildingPool = new List<(GameObject prefab, BuildingType type)>();
        if (marketPrefab != null)    buildingPool.Add((marketPrefab,    BuildingType.Market));
        if (banditDenPrefab != null) buildingPool.Add((banditDenPrefab, BuildingType.BanditDen));
        if (shrinePrefab != null)    buildingPool.Add((shrinePrefab,    BuildingType.Shrine));
        if (wanderingGamblerPrefab != null) buildingPool.Add((wanderingGamblerPrefab, BuildingType.WanderingGambler));

        if (buildingPool.Count == 0) return;

        // Spawn every building, each in a unique random city
        foreach (var (prefab, type) in buildingPool)
        {
            for (int attempt = availableMarkers.Count - 1; attempt >= 0; attempt--)
            {
                int index = Random.Range(0, availableMarkers.Count);
                Transform marker = availableMarkers[index];
                availableMarkers.RemoveAt(index);

                string cityName = marker.parent.parent.name.Replace("City_", "");
                if (usedCities.Contains(cityName) || towerCities.Contains(cityName)) continue;

                GameObject building = Instantiate(prefab, marker.position, Quaternion.Euler(0, 90, 0));
                building.transform.SetParent(marker.parent);
                building.tag = "Building";

                if (type == BuildingType.WanderingGambler)
                    building.transform.localScale = Vector3.one * 0.25f;
                else if (type == BuildingType.BanditDen)
                    building.transform.localScale = Vector3.one * 0.5f;

                BuildingInteraction bi = building.GetComponent<BuildingInteraction>();
                if (bi == null) bi = building.AddComponent<BuildingInteraction>();
                bi.buildingType = type;

                buildingCities.Add(cityName);
                usedCities.Add(cityName);
                break;
            }
        }
    }

    void SpawnChests()
    {
        usedCities.Clear();

        for (int i = 0; i < chestCount && availableMarkers.Count > 0;)
        {
            int index = Random.Range(0, availableMarkers.Count);
            Transform marker = availableMarkers[index];
            availableMarkers.RemoveAt(index);

            string cityName = marker.parent.parent.name.Replace("City_", "");

            if (usedCities.Contains(cityName) || towerCities.Contains(cityName) || buildingCities.Contains(cityName))
                continue;

            GameObject chest = Instantiate(chestPrefab, marker.position, Quaternion.Euler(0, 270, 0));
            chest.transform.SetParent(marker.parent);

            chestCities.Add(cityName);
            usedCities.Add(cityName);
            i++;
        }
    }



}
