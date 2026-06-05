using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    public static AbilityUI Instance;

    public GameObject panel;
    public Button toggleBtn;
    public Button bribeBtn;
    public Button ambushBtn;
    public Button teleportBtn;
    public Button fortifyBtn;

    public TMP_Text bribeCostText;
    public TMP_Text ambushCostText;
    public TMP_Text teleportCostText;
    public TMP_Text fortifyCostText;
    public TMP_Text goldInfoText;

    private PlayerMover currentPlayer;
    private PlayerMover opponentPlayer;
    private bool usedThisTurn = false;

    void Awake() { Instance = this; }

    bool _started = false;
    void Start() => EnsureStarted();

    void EnsureStarted()
    {
        if (_started) return;
        _started = true;

        if (panel != null) panel.SetActive(false);
        if (toggleBtn != null)
        {
            toggleBtn.gameObject.SetActive(false);
            toggleBtn.onClick.AddListener(() => panel.SetActive(!panel.activeSelf));
        }
        bribeBtn?.onClick.AddListener(()    => TryUse(AbilityType.Bribe));
        ambushBtn?.onClick.AddListener(()   => TryUse(AbilityType.Ambush));
        teleportBtn?.onClick.AddListener(() => TryUse(AbilityType.Teleport));
        fortifyBtn?.onClick.AddListener(()  => TryUse(AbilityType.Fortify));
    }

    public void ShowForPlayer(PlayerMover player, PlayerMover opponent)
    {
        Instance = this;
        gameObject.SetActive(true);
        EnsureStarted();
        currentPlayer  = player;
        opponentPlayer = opponent;
        usedThisTurn   = false;

        AbilityManager mgr = player.GetComponent<AbilityManager>();
        mgr?.OnTurnStart();

        RefreshUI();
        if (toggleBtn != null)
        {
            panel.SetActive(false);
            toggleBtn.gameObject.SetActive(true);
        }
        else
        {
            panel.SetActive(true);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        if (toggleBtn != null) toggleBtn.gameObject.SetActive(false);
        currentPlayer  = null;
        opponentPlayer = null;
    }

    void TryUse(AbilityType type)
    {
        if (usedThisTurn) return;

        AbilityManager mgr = currentPlayer?.GetComponent<AbilityManager>();
        if (mgr == null) return;

        bool success = mgr.UseAbility(type, currentPlayer, opponentPlayer);
        if (!success) return;

        usedThisTurn = true;

        if (type == AbilityType.Teleport)
            panel.SetActive(false);
        else
            DisableButtons();

        RefreshUI();
    }

    void DisableButtons()
    {
        if (bribeBtn    != null) bribeBtn.interactable    = false;
        if (ambushBtn   != null) ambushBtn.interactable   = false;
        if (teleportBtn != null) teleportBtn.interactable = false;
        if (fortifyBtn  != null) fortifyBtn.interactable  = false;
    }

    public void RefreshUI()
    {
        AbilityManager mgr = currentPlayer?.GetComponent<AbilityManager>();
        if (mgr == null) return;

        Inventory inv = currentPlayer.GetComponent<Inventory>();
        int gold = inv != null ? inv.GetItemCount(Inventory.ItemType.Gold) : 0;
        if (goldInfoText != null) goldInfoText.text = "Gold: " + gold;

        SetChargeText(bribeCostText,    bribeBtn,    mgr, AbilityType.Bribe);
        SetChargeText(ambushCostText,   ambushBtn,   mgr, AbilityType.Ambush);
        SetChargeText(teleportCostText, teleportBtn, mgr, AbilityType.Teleport);
        SetChargeText(fortifyCostText,  fortifyBtn,  mgr, AbilityType.Fortify);
    }

    void SetChargeText(TMP_Text txt, Button btn, AbilityManager mgr, AbilityType type)
    {
        int count = mgr.GetChargeCount(type);
        bool hasCharge = count > 0;

        if (txt != null)
        {
            txt.text  = hasCharge ? "x" + count : "—";
            txt.color = hasCharge ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }

        if (btn != null && !usedThisTurn)
            btn.interactable = hasCharge;
    }
}
