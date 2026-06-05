using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamblerUIOn : MonoBehaviour
{
    public static GamblerUIOn Instance;
    public static bool IsOpen { get; private set; }

    public GameObject panel;
    public GameObject dicePlace;
    public Button oddBtn;
    public Button evenBtn;
    public Button leaveBtn;
    public TMP_Text resultText;
    public TMP_Text goldText;
    public TMP_Text diceText;

    private int currentActorNum;
    private InventoryOn currentInventory;
    private bool hasGambled;
    private int _pendingRoll = -1;
    private bool _pendingWon;
    private bool _spinDone;

    void Awake() { Instance = this; }

    bool _started = false;
    void Start() => EnsureStarted();

    void EnsureStarted()
    {
        if (_started) return;
        _started = true;

        if (panel == null) { Debug.LogError("[GamblerUIOn] panel is null!"); return; }
        panel.SetActive(false);
        if (dicePlace != null) dicePlace.SetActive(false);

        if (oddBtn   == null) oddBtn   = panel.transform.Find("OddBtn")  ?.GetComponent<Button>();
        if (evenBtn  == null) evenBtn  = panel.transform.Find("EvenBtn") ?.GetComponent<Button>();
        if (leaveBtn == null) leaveBtn = panel.transform.Find("LeaveBtn")?.GetComponent<Button>();

        oddBtn?.onClick.AddListener(() => OnChoose(true));
        evenBtn?.onClick.AddListener(() => OnChoose(false));
        leaveBtn?.onClick.AddListener(Leave);
    }

    public void OpenGambler(InventoryOn inv, int actorNum)
    {
        gameObject.SetActive(true);
        EnsureStarted();
        currentInventory = inv;
        currentActorNum  = actorNum;
        hasGambled = false;
        _pendingRoll = -1;
        _spinDone = false;
        IsOpen = true;

        inv.OnInventoryChanged += RefreshGoldText;
        if (resultText != null) resultText.text = "";
        if (diceText   != null) diceText.text   = "?";
        if (leaveBtn != null) { leaveBtn.gameObject.SetActive(true); leaveBtn.interactable = true; }
        if (oddBtn   != null) oddBtn.interactable  = true;
        if (evenBtn  != null) evenBtn.interactable  = true;
        RefreshGoldText();

        if (dicePlace != null) dicePlace.SetActive(true);
        panel.SetActive(true);
    }

    void OnChoose(bool choseOdd)
    {
        if (currentInventory.GetItemCount(InventoryOn.ItemType.Gold) < 5)
        { if (resultText != null) resultText.text = "Not enough gold!"; return; }

        hasGambled = true;
        if (oddBtn   != null) oddBtn.interactable   = false;
        if (evenBtn  != null) evenBtn.interactable   = false;
        if (leaveBtn != null) leaveBtn.interactable  = false;

        TurnGameManagerOn.Instance.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_GamblerBet),
            RpcTarget.MasterClient,
            currentActorNum, choseOdd);

        StartCoroutine(SpinAnimation());
    }

    IEnumerator SpinAnimation()
    {
        _spinDone = false;
        float duration = 2.0f, elapsed = 0f, interval = 0.05f;
        while (elapsed < duration)
        {
            if (diceText != null) diceText.text = Random.Range(1, 7).ToString();
            float t = elapsed / duration;
            interval = Mathf.Lerp(0.05f, 0.35f, t * t);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
        _spinDone = true;
        ApplyPendingResult();
    }

    // Called via RPC from TurnGameManagerOn on master after roll
    public void ShowResult(int roll, bool won)
    {
        _pendingRoll = roll;
        _pendingWon  = won;
        if (_spinDone) ApplyPendingResult();
        // else: animation will call ApplyPendingResult when it finishes
    }

    void ApplyPendingResult()
    {
        if (_pendingRoll < 0) return;
        int roll = _pendingRoll;
        bool won = _pendingWon;
        _pendingRoll = -1;

        if (diceText != null) diceText.text = roll.ToString();
        string rolled = roll % 2 != 0 ? "Odd" : "Even";
        if (resultText != null)
            resultText.text = won
                ? $"Rolled {roll} ({rolled})\nYOU WIN +5g!"
                : $"Rolled {roll} ({rolled})\nYOU LOSE -5g!";
        if (leaveBtn != null) leaveBtn.interactable = true;
    }

    void RefreshGoldText()
    {
        if (goldText != null && currentInventory != null)
            goldText.text = "Gold: " + currentInventory.GetItemCount(InventoryOn.ItemType.Gold);
    }

    void Leave()
    {
        IsOpen = false;
        StopAllCoroutines();
        panel.SetActive(false);
        if (dicePlace != null) dicePlace.SetActive(false);
        if (currentInventory != null) currentInventory.OnInventoryChanged -= RefreshGoldText;

        TurnGameManagerOn.Instance.photonView.RPC(
            nameof(TurnGameManagerOn.RPC_BuildingClosed),
            RpcTarget.MasterClient,
            currentActorNum);
        currentInventory = null;
    }
}
