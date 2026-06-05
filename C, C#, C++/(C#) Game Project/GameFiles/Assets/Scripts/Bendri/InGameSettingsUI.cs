using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class InGameSettingsUI : MonoBehaviourPunCallbacks
{
    public GameObject settingsPanel;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        settingsPanel.SetActive(false); // pradžioje paslėptas
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // ===== OFFLINE  =====
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ===== ONLINE =====
    public void QuitToMainMenuOnline()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
