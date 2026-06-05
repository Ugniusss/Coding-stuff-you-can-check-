using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUIOn : MonoBehaviour
{
    public static MarketUIOn Instance;
    public static bool IsOpen { get; private set; }

    public GameObject panel;
    public Button buySwordBtn;
    public Button buyShieldBtn;
    public Button buyBowBtn;
    public Button buyAbilityBtn;
    public Button closeBtn;
    public TMP_Text goldText;
    public TMP_Text notificationText;

    private InventoryOn currentInventory;
    private int currentActorNum;
    private AbilityType? currentAbilityStock;
    private const float AbilityStockChance = 0.40f;
    private const int AbilityCost = 4;
    private const int ItemCost = 3;

    void Awake() { Instance = this; }

    bool _started = false;
    void Start() => EnsureStarted();

    void EnsureStarted()
    {
        if (_started) return;
        _started = true;

        if (panel == null) { Debug.LogError("[MarketUIOn] panel is null!"); return; }

        buySwordBtn  = FindBtn("Sword")           ?? buySwordBtn;
        buyShieldBtn = FindBtn("Shield")          ?? buyShieldBtn;
        buyBowBtn    = FindBtn("Bow")             ?? buyBowBtn;
        buyAbilityBtn= FindBtn("PlaceForAbility") ?? buyAbilityBtn;
        closeBtn     = FindBtn("CloseBtn")        ?? closeBtn;

        if (notificationText == null)
            foreach (var t in GetComponentsInChildren<TMP_Text>(true))
                if (t.name == "NotificationText") { notificationText = t; break; }

        if (notificationText != null) notificationText.gameObject.SetActive(false);
        panel.SetActive(false);

        buySwordBtn?.onClick.AddListener(() => Buy(InventoryOn.ItemType.Sword));
        buyShieldBtn?.onClick.AddListener(() => Buy(InventoryOn.ItemType.Shield));
        buyBowBtn?.onClick.AddListener(() => Buy(InventoryOn.ItemType.Bow));
        buyAbilityBtn?.onClick.AddListener(BuyAbility);
        closeBtn?.onClick.AddListener(Close);
    }

    Button FindBtn(string n)
    {
        foreach (var b in GetComponentsInChildren<Button>(true))
            if (b.name == n) return b;
        return null;
    }

    public void OpenMarket(InventoryOn inv, int actorNum)
    {
        gameObject.SetActive(true);
        EnsureStarted();
        currentInventory = inv;
        currentActorNum  = actorNum;
        IsOpen = true;

        inv.OnInventoryChanged += RefreshGoldText;
        RollAbilityStock();
        RefreshGoldText();
        if (notificationText != null) notificationText.gameObject.SetActive(false);
        panel.SetActive(true);
    }

    void RollAbilityStock()
    {
        if (Random.value < AbilityStockChance)
        {
            var vals = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
            currentAbilityStock = vals[Random.Range(0, vals.Length)];
            if (buyAbilityBtn != null)
            {
                var lbl = buyAbilityBtn.GetComponentInChildren<TMP_Text>();
                if (lbl != null) lbl.text = $"{currentAbilityStock} — {AbilityCost}g";
                buyAbilityBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            currentAbilityStock = null;
            buyAbilityBtn?.gameObject.SetActive(false);
        }
    }

    void Buy(InventoryOn.ItemType item)
    {
        if (currentInventory == null) return;
        if (currentInventory.GetItemCount(InventoryOn.ItemType.Gold) < ItemCost)
        { ShowNotification("Not enough gold!"); return; }

        TurnGameManagerOn.Instance.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_MarketBuyItem),
            RpcTarget.MasterClient,
            currentActorNum, (int)item, ItemCost);
    }

    void BuyAbility()
    {
        if (!currentAbilityStock.HasValue || currentInventory == null) return;
        if (currentInventory.GetItemCount(InventoryOn.ItemType.Gold) < AbilityCost)
        { ShowNotification("Not enough gold!"); return; }

        TurnGameManagerOn.Instance.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_MarketBuyAbility),
            RpcTarget.MasterClient,
            currentActorNum, (int)currentAbilityStock.Value, AbilityCost);

        currentAbilityStock = null;
        buyAbilityBtn?.gameObject.SetActive(false);
    }

    void ShowNotification(string msg)
    {
        if (notificationText == null) return;
        StopAllCoroutines();
        notificationText.text = msg;
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideAfter(2f));
    }

    IEnumerator HideAfter(float t) { yield return new WaitForSeconds(t); if (notificationText != null) notificationText.gameObject.SetActive(false); }

    void RefreshGoldText()
    {
        if (goldText != null && currentInventory != null)
            goldText.text = "Gold: " + currentInventory.GetItemCount(InventoryOn.ItemType.Gold);
    }

    void Close()
    {
        IsOpen = false;
        panel.SetActive(false);
        if (currentInventory != null) currentInventory.OnInventoryChanged -= RefreshGoldText;
        TurnGameManagerOn.Instance.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_BuildingClosed),
            RpcTarget.MasterClient,
            currentActorNum);
        currentInventory = null;
    }
}
