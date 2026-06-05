using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIOn : MonoBehaviour
{
    public static AbilityUIOn Instance;

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

    private string _pendingAbility = "None";
    private bool _usedThisTurn = false;

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

    public void ShowForTurn()
    {
        gameObject.SetActive(true);
        EnsureStarted();
        _pendingAbility = "None";
        _usedThisTurn = false;
        TeleportUI.OnlinePendingCity = null;

        // Grab local player's AbilityManager
        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var mover))
        {
            var mgr = mover.GetComponent<AbilityManager>();
            mgr?.OnTurnStart();
        }

        RefreshUI();
        if (toggleBtn != null)
        {
            panel.SetActive(false);
            toggleBtn.gameObject.SetActive(true);
        }
        else panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
        if (toggleBtn != null) toggleBtn.gameObject.SetActive(false);
        _pendingAbility = "None";
        _usedThisTurn = false;
        TeleportUI.OnlinePendingCity = null;
    }

    void TryUse(AbilityType type)
    {
        if (_usedThisTurn) return;

        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var mover)) return;
        AbilityManager mgr = mover.GetComponent<AbilityManager>();
        if (mgr == null || !mgr.HasCharge(type)) return;

        _usedThisTurn = true;

        if (type == AbilityType.Teleport)
        {
            // Open TeleportUI in Online mode
            TeleportUI.OnlinePendingCity = null;
            TeleportUI ui = TeleportUI.Instance;
            if (ui == null)
            {
                var all = Resources.FindObjectsOfTypeAll<TeleportUI>();
                if (all.Length > 0) { ui = all[0]; ui.gameObject.SetActive(true); }
            }
            if (ui != null) ui.OpenPicker(null); // null → OnlinePendingCity mode
            panel.SetActive(false);
        }
        else
        {
            _pendingAbility = type.ToString();
            DisableButtons();
            StartCoroutine(ShowConfirmation(type + " queued!"));
        }
        RefreshUI();
    }

    void DisableButtons()
    {
        if (bribeBtn    != null) bribeBtn.interactable    = false;
        if (ambushBtn   != null) ambushBtn.interactable   = false;
        if (teleportBtn != null) teleportBtn.interactable = false;
        if (fortifyBtn  != null) fortifyBtn.interactable  = false;
    }

    IEnumerator ShowConfirmation(string msg)
    {
        var go = new GameObject("ConfirmPopup");
        go.transform.SetParent(panel.transform, false);
        go.transform.SetAsLastSibling();

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.offsetMin = new Vector2(4f, 4f);
        rt.offsetMax = new Vector2(-4f, 54f);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.55f, 0.1f, 0.9f);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var trt = textGo.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
        var txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = msg;
        txt.fontSize = 22;
        txt.fontStyle = FontStyles.Bold;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        yield return new WaitForSeconds(2f);
        if (go != null) Destroy(go);
    }

    public string ConsumePendingAbility()
    {
        // If teleport: city must have been picked
        if (_usedThisTurn && _pendingAbility == "None" && TeleportUI.OnlinePendingCity != null)
            _pendingAbility = "Teleport:" + TeleportUI.OnlinePendingCity;

        string result = _pendingAbility;
        _pendingAbility = "None";
        TeleportUI.OnlinePendingCity = null;
        return result;
    }

    public void RefreshUI()
    {
        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var mover)) return;

        AbilityManager mgr = mover.GetComponent<AbilityManager>();
        if (mgr == null) return;

        InventoryOn inv = mover.GetComponent<InventoryOn>();
        int gold = inv != null ? inv.GetItemCount(InventoryOn.ItemType.Gold) : 0;
        if (goldInfoText != null) goldInfoText.text = "Gold: " + gold;

        SetChargeText(bribeCostText,    bribeBtn,    mgr, AbilityType.Bribe);
        SetChargeText(ambushCostText,   ambushBtn,   mgr, AbilityType.Ambush);
        SetChargeText(teleportCostText, teleportBtn, mgr, AbilityType.Teleport);
        SetChargeText(fortifyCostText,  fortifyBtn,  mgr, AbilityType.Fortify);
    }

    void SetChargeText(TMP_Text txt, Button btn, AbilityManager mgr, AbilityType type)
    {
        int count = mgr.GetChargeCount(type);
        bool has = count > 0;
        if (txt != null) { txt.text = has ? "x" + count : "—"; txt.color = has ? Color.white : new Color(0.5f, 0.5f, 0.5f); }
        if (btn != null && !_usedThisTurn) btn.interactable = has;
    }
}
