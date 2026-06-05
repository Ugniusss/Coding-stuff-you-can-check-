using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject modePanel;
    public GameObject mainPanel;

    public void StartGame()
    {
        modePanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
