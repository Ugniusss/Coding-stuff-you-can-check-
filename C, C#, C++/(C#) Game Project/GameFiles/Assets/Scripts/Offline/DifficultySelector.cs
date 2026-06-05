using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    public static DifficultySelector Instance;

    public GameObject panel;
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;
    public BotAI botAI;

    void Awake() => Instance = this;

    void Start()
    {
        panel.SetActive(true);
        easyBtn.onClick.AddListener(()   => Select(BotDifficulty.Easy));
        mediumBtn.onClick.AddListener(() => Select(BotDifficulty.Medium));
        hardBtn.onClick.AddListener(()   => Select(BotDifficulty.Hard));
    }

    void Select(BotDifficulty difficulty)
    {
        if (botAI != null) botAI.difficulty = difficulty;
        panel.SetActive(false);
        ClassSelectionManager.Instance?.ShowPanel();
    }
}
