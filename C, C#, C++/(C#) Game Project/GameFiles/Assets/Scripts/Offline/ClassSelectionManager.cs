using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectionManager : MonoBehaviour
{
    public static ClassSelectionManager Instance;

    [Header("Overlay")]
    public GameObject selectionPanel;
    public GameObject choiceManagerObject;
    public bool startHidden = false;  // Set true in OfflineBot scene (DifficultySelector shows first)
    public bool p2IsBot     = false;  // Set true in OfflineBot scene (P2 panel hidden, random class)

    [Header("Legend")]
    public Button legendBtn;
    public GameObject classInfoPanel;

    [Header("Player 1")]
    public Button p1MerchantBtn;
    public Button p1WarriorBtn;
    public Button p1WandererBtn;
    public Button p1ReadyBtn;
    public TMP_Text p1StatusText;

    [Header("Player 2")]
    public Button p2MerchantBtn;
    public Button p2WarriorBtn;
    public Button p2WandererBtn;
    public Button p2ReadyBtn;
    public TMP_Text p2StatusText;

    private PlayerClass p1Class = PlayerClass.Merchant;
    private PlayerClass p2Class = PlayerClass.Merchant;
    private bool p1Ready = false;
    private bool p2Ready = false;

    void Awake() => Instance = this;

    void Start()
    {
        if (!startHidden)
            selectionPanel.SetActive(true);
        else
            selectionPanel.SetActive(false);

        if (choiceManagerObject != null)
            choiceManagerObject.SetActive(false);

        if (legendBtn != null && classInfoPanel != null)
        {
            classInfoPanel.SetActive(false);
            legendBtn.onClick.AddListener(() => classInfoPanel.SetActive(!classInfoPanel.activeSelf));

            // Wire up Close button inside the info panel
            var closeBtn = classInfoPanel.transform.Find("CloseBtn")?.GetComponent<Button>();
            if (closeBtn != null)
                closeBtn.onClick.AddListener(() => classInfoPanel.SetActive(false));
        }

        p1MerchantBtn.onClick.AddListener(() => SelectClass(1, PlayerClass.Merchant));
        p1WarriorBtn.onClick.AddListener(() => SelectClass(1, PlayerClass.Warrior));
        p1WandererBtn.onClick.AddListener(() => SelectClass(1, PlayerClass.Wanderer));
        p1ReadyBtn.onClick.AddListener(() => SetReady(1));

        if (!p2IsBot)
        {
            p2MerchantBtn.onClick.AddListener(() => SelectClass(2, PlayerClass.Merchant));
            p2WarriorBtn.onClick.AddListener(() => SelectClass(2, PlayerClass.Warrior));
            p2WandererBtn.onClick.AddListener(() => SelectClass(2, PlayerClass.Wanderer));
            p2ReadyBtn.onClick.AddListener(() => SetReady(2));
        }
        else
        {
            // Hide P2 panel in bot mode
            p2MerchantBtn?.transform.parent?.gameObject.SetActive(false);
        }

        HighlightClass(1, p1Class);
        if (!p2IsBot) HighlightClass(2, p2Class);
    }

    // Called by DifficultySelector when it's done
    public void ShowPanel() => selectionPanel.SetActive(true);

    void SelectClass(int player, PlayerClass chosen)
    {
        if (player == 1) { p1Class = chosen; p1Ready = false; }
        else             { p2Class = chosen; p2Ready = false; }

        HighlightClass(player, chosen);
        HighlightReady(player, false);
    }

    void SetReady(int player)
    {
        if (player == 1) p1Ready = true;
        else             p2Ready = true;

        HighlightReady(player, true);

        if (p2IsBot && p1Ready)
        {
            // Auto-assign random class to bot
            var values = (PlayerClass[])System.Enum.GetValues(typeof(PlayerClass));
            p2Class = values[Random.Range(0, values.Length)];
            p2Ready = true;
        }

        if (p1Ready && p2Ready)
            StartGame();
    }

    void HighlightReady(int player, bool ready)
    {
        Button readyBtn = player == 1 ? p1ReadyBtn : p2ReadyBtn;
        if (readyBtn == null) return;

        // Change Outline color: green when ready, normal when not
        Transform outline = readyBtn.transform.Find("Back/Green/Outline");
        if (outline != null)
        {
            Image img = outline.GetComponent<Image>();
            if (img != null)
                img.color = ready ? new Color(0.3f, 0.95f, 0.35f, 1f) : new Color(1f, 0.87f, 0f, 0.35f);
        }

        // Also tint the button root image
        Image btnImg = readyBtn.GetComponent<Image>();
        if (btnImg != null)
            btnImg.color = ready ? new Color(0.3f, 0.85f, 0.35f, 0.45f) : new Color(1f, 1f, 1f, 0f);
    }

    void HighlightClass(int player, PlayerClass selected)
    {
        Button[] btns = player == 1
            ? new[] { p1MerchantBtn, p1WarriorBtn, p1WandererBtn }
            : new[] { p2MerchantBtn, p2WarriorBtn, p2WandererBtn };

        PlayerClass[] classes = { PlayerClass.Merchant, PlayerClass.Warrior, PlayerClass.Wanderer };

        for (int i = 0; i < btns.Length; i++)
        {
            bool isSelected = classes[i] == selected;

            // Change the Outline border color: green when selected, dim yellow when not
            Transform outline = btns[i].transform.Find("Color/Yellow/Outline");
            if (outline != null)
            {
                Image img = outline.GetComponent<Image>();
                if (img != null)
                    img.color = isSelected ? new Color(0.3f, 0.95f, 0.35f, 1f) : new Color(1f, 0.87f, 0f, 0.35f);
            }

            // Also tint the button root image as a background highlight
            Image btnImg = btns[i].GetComponent<Image>();
            if (btnImg != null)
                btnImg.color = isSelected ? new Color(0.3f, 0.85f, 0.35f, 0.45f) : new Color(1f, 1f, 1f, 0f);
        }
    }

    void StartGame()
    {
        if (GameManager.Instance?.players != null)
        {
            ApplyClassToPlayer(GameManager.Instance.players[0], p1Class);
            ApplyClassToPlayer(GameManager.Instance.players[1], p2Class);
        }

        selectionPanel.SetActive(false);
        if (choiceManagerObject != null)
            choiceManagerObject.SetActive(true);
    }

    void ApplyClassToPlayer(PlayerMover mover, PlayerClass cls)
    {
        if (mover == null) return;
        mover.playerClass = cls;

        Inventory inv = mover.GetComponent<Inventory>();
        if (inv == null) return;

        int startGold = ClassData.StartingGold(cls);
        if (startGold > 0)
            inv.ModifyItem(Inventory.ItemType.Gold, startGold);

        if (ClassData.StartsWithWeapons(cls))
        {
            inv.ModifyItem(Inventory.ItemType.Sword, 1);
            inv.ModifyItem(Inventory.ItemType.Shield, 1);
            inv.ModifyItem(Inventory.ItemType.Bow, 1);
        }
    }
}
