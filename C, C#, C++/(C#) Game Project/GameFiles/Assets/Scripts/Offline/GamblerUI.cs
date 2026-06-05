using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamblerUI : MonoBehaviour
{
    public static GamblerUI Instance;
    public static bool IsOpen { get; private set; }

    public GameObject panel;
    public GameObject dicePlace;  // sibling of panel — hidden with it
    public GameObject dimmer;
    public Button oddBtn;
    public Button evenBtn;
    public Button leaveBtn;
    public TMP_Text resultText;
    public TMP_Text goldText;
    public TMP_Text diceText;   // TMP_Text inside DicePlace

    private Inventory currentInventory;
    private int currentPlayerIndex;
    private BuildingInteraction currentBuilding;
    private bool hasGambled = false;

    void Awake() { Instance = this; }

    bool _started = false;
    void Start() => EnsureStarted();

    void EnsureStarted()
    {
        if (_started) return;
        _started = true;

        if (dimmer != null) dimmer.SetActive(false);
        if (dicePlace != null) dicePlace.SetActive(false);
        if (panel != null) panel.SetActive(false);

        if (oddBtn   == null) oddBtn   = panel?.transform.Find("OddBtn")  ?.GetComponent<Button>();
        if (evenBtn  == null) evenBtn  = panel?.transform.Find("EvenBtn") ?.GetComponent<Button>();
        if (leaveBtn == null) leaveBtn = panel?.transform.Find("LeaveBtn")?.GetComponent<Button>();

        oddBtn?.onClick.AddListener(() => OnChoose(true));
        evenBtn?.onClick.AddListener(() => OnChoose(false));
        leaveBtn?.onClick.AddListener(Leave);
    }

    public void OpenGambler(Inventory inv, int playerIndex, BuildingInteraction building)
    {
        Instance = this;
        gameObject.SetActive(true);
        EnsureStarted();
        currentInventory = inv;
        currentPlayerIndex = playerIndex;
        currentBuilding = building;
        hasGambled = false;

        resultText.text = "";
        if (diceText != null) diceText.text = "?";
        leaveBtn.gameObject.SetActive(true);
        oddBtn.interactable = true;
        evenBtn.interactable = true;
        RefreshGoldText();

        IsOpen = true;
        if (dimmer != null) dimmer.SetActive(true);
        if (dicePlace != null) dicePlace.SetActive(true);
        panel.SetActive(true);
    }

    void OnChoose(bool choseOdd)
    {
        if (currentInventory.GetItemCount(Inventory.ItemType.Gold) < 5)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        hasGambled = true;
        oddBtn.interactable = false;
        evenBtn.interactable = false;
        leaveBtn.interactable = false;

        StartCoroutine(RollDice(choseOdd));
    }

    IEnumerator RollDice(bool choseOdd)
    {
        // Spin fast then slow down
        float duration = 2.0f;
        float elapsed = 0f;
        float interval = 0.05f; // start fast

        while (elapsed < duration)
        {
            if (diceText != null)
                diceText.text = Random.Range(1, 7).ToString();

            // Slow down as we approach the end
            float t = elapsed / duration;
            interval = Mathf.Lerp(0.05f, 0.35f, t * t);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // Final roll
        int roll = Random.Range(1, 7);
        if (diceText != null) diceText.text = roll.ToString();

        bool isOdd = roll % 2 != 0;
        bool won = choseOdd == isOdd;

        yield return new WaitForSeconds(0.4f);

        currentInventory.ModifyItem(Inventory.ItemType.Gold, won ? 5 : -5);

        string rolled = isOdd ? "Odd" : "Even";
        resultText.text = won
            ? $"Rolled {roll} ({rolled})\nYOU WIN +5g!"
            : $"Rolled {roll} ({rolled})\nYOU LOSE -5g!";

        GameLog.Instance?.Add($"P{currentPlayerIndex + 1} gambled: rolled {roll}, {(won ? "won +5g" : "lost -5g")}");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(won ? AudioManager.Instance.chest : AudioManager.Instance.tower);

        RefreshGoldText();
        leaveBtn.interactable = true;
    }

    void RefreshGoldText()
    {
        if (goldText != null && currentInventory != null)
            goldText.text = "Gold: " + currentInventory.GetItemCount(Inventory.ItemType.Gold);
    }

    void Leave()
    {
        IsOpen = false;
        StopAllCoroutines();
        panel.SetActive(false);
        if (dicePlace != null) dicePlace.SetActive(false);
        if (dimmer != null) dimmer.SetActive(false);

        if (hasGambled)
            currentBuilding?.RegisterGamblerVisit(currentPlayerIndex);
        currentBuilding = null;
    }
}
