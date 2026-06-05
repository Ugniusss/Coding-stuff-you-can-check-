using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public Transform citiesParent;

    void Start()
    {
        SpawnTwoPlayers();
    }

    void SpawnTwoPlayers()
    {
        List<Transform> allCities = new List<Transform>();
        foreach (Transform city in citiesParent)
        {
            allCities.Add(city);
        }

        if (allCities.Count < 2)
        {
            //Debug.LogError("Need at least 2 cities to spawn players.");
            return;
        }

        int firstIndex = Random.Range(0, allCities.Count);
        int secondIndex;
        do { secondIndex = Random.Range(0, allCities.Count); }
        while (secondIndex == firstIndex);

        Transform city1 = allCities[firstIndex];
        Transform city2 = allCities[secondIndex];

        GameObject player1 = SpawnPlayerAtMarker(city1, player1Prefab, "Player1");
        GameObject player2 = SpawnPlayerAtMarker(city2, player2Prefab, "Player2");

        InventoryUI[] uiPanels = FindObjectsByType<InventoryUI>(FindObjectsSortMode.None);
        foreach (InventoryUI ui in uiPanels)
        {
            if (ui.name.Contains("1"))
                ui.Initialize(player1.GetComponent<Inventory>());
            else if (ui.name.Contains("2"))
                ui.Initialize(player2.GetComponent<Inventory>());
        }

        PlayerMover mover1 = player1.GetComponent<PlayerMover>();
        if (mover1 != null)
        {
            mover1.currentCityName = city1.name.Replace("City_", ""); 
            mover1.citiesParent = citiesParent;
            mover1.redPathsParent = GameObject.Find("RedPaths")?.transform;
            mover1.bluePathsParent = GameObject.Find("BluePaths")?.transform;
            mover1.yellowPathsParent = GameObject.Find("YellowPaths")?.transform;
        }

        PlayerMover mover2 = player2.GetComponent<PlayerMover>();
        if (mover2 != null)
        {
            mover2.currentCityName = city2.name.Replace("City_", "");
            mover2.citiesParent = citiesParent;
            mover2.redPathsParent = GameObject.Find("RedPaths")?.transform;
            mover2.bluePathsParent = GameObject.Find("BluePaths")?.transform;
            mover2.yellowPathsParent = GameObject.Find("YellowPaths")?.transform;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.players = new PlayerMover[2];
            GameManager.Instance.players[0] = mover1;
            GameManager.Instance.players[1] = mover2;
           // Debug.Log("Assigned both players to GameManager.");
        }
        else
        {
           // Debug.LogWarning("GameManager.Instance is null — players not assigned.");
        }
    }

    GameObject SpawnPlayerAtMarker(Transform city, GameObject playerPrefab, string playerName)
    {
        Transform marker = city.Find("PlayerPositions/VisualMarker");
        if (marker != null)
        {
            GameObject player = Instantiate(playerPrefab, marker.position, Quaternion.identity);
            player.name = playerName;
            player.tag = "Player";
            return player;
        }
        else
        {
            //Debug.LogWarning($"No VisualMarker found in {city.name}");
            return null;
        }
    }
}
