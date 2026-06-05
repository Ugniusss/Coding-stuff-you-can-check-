using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public static InfoPanel Instance;

    public GameObject panel;
    public Button infoButton;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void Start()
    {
        if (infoButton != null)
            infoButton.onClick.AddListener(Toggle);

        // Also wire CloseButton inside the panel if it exists
        var closeBtn = panel?.transform.Find("Panel/CloseButton")?.GetComponent<Button>();
        if (closeBtn != null)
            closeBtn.onClick.AddListener(Hide);
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
