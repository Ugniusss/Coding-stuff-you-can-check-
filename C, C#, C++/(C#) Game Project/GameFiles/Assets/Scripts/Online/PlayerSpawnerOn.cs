using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawnerOn : MonoBehaviourPunCallbacks
{
    public Transform citiesParent;
    public GameObject playerPrefab;

    private bool hasSpawned;

    void Start()
    {
        Debug.Log("[Spawner] Start");
        TryInit();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[Spawner] OnJoinedRoom");
        TryInit();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[Spawner] OnPlayerEnteredRoom: " + newPlayer.ActorNumber);
        TryInit();
    }

    void TryInit()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("[Spawner] Not in room");
            return;
        }

        int count = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("[Spawner] PlayerCount = " + count);

        if (count < 2)
        {
            Debug.Log("[Spawner] Waiting for second player");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[Spawner] Not MasterClient");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("[Spawner] photonView is NULL (add PhotonView!)");
            return;
        }

        AssignSpawnCities();
    }

    void AssignSpawnCities()
    {
        if (citiesParent == null)
        {
            Debug.LogError("[Spawner] citiesParent NULL");
            return;
        }

        int cityCount = citiesParent.childCount;
        Debug.Log("[Spawner] City count = " + cityCount);

        if (cityCount < 2)
        {
            Debug.LogError("[Spawner] Not enough cities");
            return;
        }

        int cityA = Random.Range(0, cityCount);
        int cityB;
        do { cityB = Random.Range(0, cityCount); }
        while (cityB == cityA);

        Debug.Log($"[Spawner] Chosen cities: {cityA}, {cityB}");

        Player[] players = PhotonNetwork.PlayerList;
        if (players == null || players.Length < 2)
        {
            Debug.LogError("[Spawner] PlayerList invalid");
            return;
        }

        photonView.RPC("RPC_Spawn", players[0], cityA);
        photonView.RPC("RPC_Spawn", players[1], cityB);
    }

    [PunRPC]
    void RPC_Spawn(int cityIndex)
    {
        Transform city = citiesParent.GetChild(cityIndex);
        Transform marker = city.Find("PlayerPositions/VisualMarker");
        if (marker == null) return;

        Vector3 spawnPos = marker.position;
        spawnPos.y += 0.1f;

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPos,
            Quaternion.identity
        );

        string cityName = city.name.Replace("City_", "");

        PlayerMoverOn mover = player.GetComponent<PlayerMoverOn>();
        if (mover != null)
        {
            mover.photonView.RPC(
                nameof(PlayerMoverOn.RPC_SetCity),
                RpcTarget.All,
                cityName
            );

            Debug.Log($"[Spawner] Set start city {cityName} for actor {PhotonNetwork.LocalPlayer.ActorNumber}");
        }
    }



}
