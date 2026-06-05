using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInputField;
    public GameObject waitingPanel;
    public GameObject joinPanel;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // prisijungia prie Photon serveriu
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Prie photon prisjungta");
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        string roomCode = GenerateRoomCode();
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomCode, options);
    }

    public void JoinRoom()
    {
        string roomCode = roomInputField.text.ToUpper();
        PhotonNetwork.JoinRoom(roomCode);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Party JOINED: " + PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            waitingPanel.SetActive(true);
        }
        else
        {
            PhotonNetwork.LoadLevel("Game"); 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < 6; i++)
        {
            code += chars[Random.Range(0, chars.Length)];
        }
        return code;
    }
}
