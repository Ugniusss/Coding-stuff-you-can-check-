using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUI : MonoBehaviour
{
    public static MarketUI Instance;
    public static bool IsOpen { get; private set; }

    public GameObject panel;
    public Button buySwordBtn;
    public Button buyShieldBtn;
    public Button buyBowBtn;
    public Button buyAbilityBtn;
    public Button closeBtn;
    public TMP_Text goldText;
    public TMP_Text notificationText;

    private Inventory currentInventory;
    private AbilityManager currentAbilityMgr;
    private BuildingInteraction currentBuilding;
    private AbilityType? currentAbilityStock;
    private int currentPlayerIndex;

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

        if (panel == null) { Debug.LogError("[MarketUI] panel is null!"); return; }

        buySwordBtn   = FindBtn("Sword")           ?? buySwordBtn;
        buyShieldBtn  = FindBtn("Shield")          ?? buyShieldBtn;
        buyBowBtn     = FindBtn("Bow")             ?? buyBowBtn;
        buyAbilityBtn = FindBtn("PlaceForAbility") ?? buyAbilityBtn;
        closeBtn      = FindBtn("CloseBtn")        ?? closeBtn;
        if (notificationText == null)
        {
            foreach (var t in panel.GetComponentsInChildren<TMP_Text>(true))
                if (t.name == "NotificationText") { notificationText = t; break; }
        }

        if (notificationText != null) notificationText.gameObject.SetActive(false);

        panel.SetActive(false);
        buySwordBtn?.onClick.AddListener(() => Buy(Inventory.ItemType.Sword));
        buyShieldBtn?.onClick.AddListener(() => Buy(Inventory.ItemType.Shield));
        buyBowBtn?.onClick.AddListener(() => Buy(Inventory.ItemType.Bow));
        buyAbilityBtn?.onClick.AddListener(BuyAbilityScroll);
        closeBtn?.onClick.AddListener(Close);

        if (buySwordBtn == null)  Debug.LogError("[MarketUI] Sword button not found");
        if (buyShieldBtn == null) Debug.LogError("[MarketUI] Shield button not found");
        if (buyBowBtn == null)    Debug.LogError("[MarketUI] Bow button not found");
        if (closeBtn == null)     Debug.LogError("[MarketUI] CloseBtn not found");
    }

    Button FindBtn(string btnName)
    {
        foreach (var b in GetComponentsInChildren<Button>(true))
            if (b.name == btnName) return b;
        return null;
    }

    public void OpenMarket(Inventory inv, int playerIndex, BuildingInteraction building)
    {
        Instance = this;
        gameObject.SetActive(true);
        EnsureStarted();
        if (panel == null) { Debug.LogError("[MarketUI] OpenMarket called but panel is null!"); return; }
        currentInventory = inv;
        currentPlayerIndex = playerIndex;
        currentBuilding = building;
        currentAbilityMgr = inv.GetComponent<AbilityManager>();

        IsOpen = true;
        RollAbilityStock();
        RefreshGoldText();
        if (notificationText != null) notificationText.gameObject.SetActive(false);
        panel.SetActive(true);
    }

    void RollAbilityStock()
    {
        if (Random.value < AbilityStockChance)
        {
            var values = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
            currentAbilityStock = values[Random.Range(0, values.Length)];
            if (buyAbilityBtn != null)
            {
                var lbl = buyAbilityBtn.GetComponentInChildren<TMP_Text>();
                if (lbl != null)
                {
                    lbl.enableWordWrapping = false;
                    lbl.overflowMode = TextOverflowModes.Ellipsis;
                    lbl.text = $"{currentAbilityStock} ({AbilityCost}g)";
                }
                buyAbilityBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            currentAbilityStock = null;
            buyAbilityBtn?.gameObject.SetActive(false);
        }
    }

    void Buy(Inventory.ItemType item)
    {
        if (currentBuilding == null || currentInventory == null) return;
        if (currentInventory.GetItemCount(Inventory.ItemType.Gold) < ItemCost) { ShowNotification("Not enough gold!"); return; }
        currentBuilding.BuyItem(currentInventory, item, currentPlayerIndex);
        RefreshGoldText();
    }

    void BuyAbilityScroll()
    {
        if (currentBuilding == null || currentInventory == null || currentAbilityMgr == null) return;
        if (!currentAbilityStock.HasValue) return;
        if (currentInventory.GetItemCount(Inventory.ItemType.Gold) < AbilityCost)
        {
            ShowNotification("Not enough gold!");
            return;
        }

        currentBuilding.BuyAbilityScroll(currentInventory, currentAbilityMgr, currentAbilityStock.Value, currentPlayerIndex);
        currentAbilityStock = null;
        buyAbilityBtn?.gameObject.SetActive(false);
        RefreshGoldText();
    }

    void ShowNotification(string message)
    {
        if (notificationText == null) return;
        StopAllCoroutines();
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideNotificationAfterDelay(2f));
    }

    IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    void RefreshGoldText()
    {
        if (goldText != null && currentInventory != null)
            goldText.text = "Gold: " + currentInventory.GetItemCount(Inventory.ItemType.Gold);
    }

    void Close()
    {
        IsOpen = false;
        panel.SetActive(false);
    }
}
