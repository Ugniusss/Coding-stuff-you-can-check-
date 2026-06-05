using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject MainButtons;
    public GameObject ModeButtons;
    public GameObject OfflineModeButtons;
    public GameObject CreateJoinUI;
    public GameObject LobbyUI;
    public GameObject SettingsUI;

    [Header("UI Components")]
    public TMP_InputField joinInputField;
    public TextMeshProUGUI lobbyCodeText;
    public GameObject P2Block;
    public GameObject WaitingBlock;


    private bool canCreateRoom = true;
    


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // prisijungia prie Photon
        HideAllPanels();
        MainButtons.SetActive(true);

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(" Prisijungta prie Photon Master Server.");
        PhotonNetwork.JoinLobby();
        canCreateRoom = true; // Optional, bet naudinga norint naudot random matchmaking
    }

    // ------------------ UI Navigation ------------------

    public void OnPlayPressed()
    {
        MainButtons.SetActive(false);
        ModeButtons.SetActive(true);
    }

    public void OnOfflinePressed()
    {
        ModeButtons.SetActive(false);
        OfflineModeButtons.SetActive(true);
    }

    public void OnVsPlayerPressed()
    {
        SceneManager.LoadScene("Offline");
    }

    public void OnBotPressed()
    {
        SceneManager.LoadScene("OfflineBot");
    }

    public void OnBackFromOfflineMode()
    {
        OfflineModeButtons.SetActive(false);
        ModeButtons.SetActive(true);
    }
    public void OnSettingsPressed()
    {
        HideAllPanels();
        SettingsUI.SetActive(true);
    }

    public void OnBackFromSettings()
    {
        HideAllPanels();
        MainButtons.SetActive(true);
    }

    public void OnOnlinePressed()
    {
        ModeButtons.SetActive(false);
        CreateJoinUI.SetActive(true);
    }

    public void OnCreateLobby()
    {
        if (!canCreateRoom)
        {
            Debug.LogWarning("⛔ Photon dar nepasiruošęs!");
            return;
        }

        string roomCode = GenerateRoomCode();
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomCode, options);
        canCreateRoom = false; // kol laukiame patvirtinimo

        
    }

    public void OnJoinLobby()
    {
        string code = joinInputField.text;
        if (!string.IsNullOrEmpty(code))
        {
            PhotonNetwork.JoinRoom(code.ToUpper());
            Debug.Log("Bandoma jungtis prie kambario: " + code);
        }
    }

    public void OnBackFromCreateJoin()
    {
        CreateJoinUI.SetActive(false);
        ModeButtons.SetActive(true);
    }

    public void OnBackFromMode()
    {
        ModeButtons.SetActive(false);
        MainButtons.SetActive(true);
    }

    public void OnBackFromLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); 
            Debug.Log("Iškviečiamas LeaveRoom()");
        }
        LobbyUI.SetActive(false);
        CreateJoinUI.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        Debug.Log(" Palikta patalpa (room). Grįžtam į Master Server...");
        PhotonNetwork.ConnectUsingSettings();
    }


    public void OnStartGamePressed() 
    {
        Debug.Log("Žaidimas paleidžiamas!");
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("[MainMenu] Not connected");
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("[MainMenu] Not in room");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[MainMenu] Only host can start the game");
            return;
        }

        Debug.Log("[MainMenu] Host loading Online scene");
        PhotonNetwork.LoadLevel("Online");
    }

    // ------------------ Photon Callbacks ------------------

    public override void OnJoinedRoom()
    {
        Debug.Log(" Prisijungta prie kambario: " + PhotonNetwork.CurrentRoom.Name);
        HideAllPanels();
        LobbyUI.SetActive(true);
        lobbyCodeText.text = PhotonNetwork.CurrentRoom.Name;

        FindObjectOfType<LobbyChat>().ClearChat();

        UpdateLobbyPlayers();
    }
    [PunRPC]
    void SyncLobbyUI()
    {
        UpdateLobbyPlayers();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("SyncLobbyUI", RpcTarget.All);
    }
   
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyPlayers();
    }

    // ------------------ Helper Methods ------------------

    private void UpdateLobbyPlayers()
    {
        if (LobbyUI == null || !LobbyUI.activeInHierarchy) return;
        if (P2Block == null || WaitingBlock == null) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            P2Block.SetActive(true);
            WaitingBlock.SetActive(false);
        }
        else
        {
            P2Block.SetActive(false);
            WaitingBlock.SetActive(true);
        }
    }

    private void HideAllPanels()
    {
        MainButtons.SetActive(false);
        ModeButtons.SetActive(false);
        if (OfflineModeButtons != null) OfflineModeButtons.SetActive(false);
        CreateJoinUI.SetActive(false);
        LobbyUI.SetActive(false);
        SettingsUI.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();

    }
    private string GenerateRoomCode()
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
