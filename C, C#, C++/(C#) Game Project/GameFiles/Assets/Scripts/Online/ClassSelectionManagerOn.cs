using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectionManagerOn : MonoBehaviourPun
{
    public static ClassSelectionManagerOn Instance;

    [Header("Panel")]
    public GameObject selectionPanel;
    public GameObject choiceManagerObject;

    [Header("Legend")]
    public Button legendBtn;
    public GameObject classInfoPanel;

    [Header("Game UI to hide during class selection")]
    public GameObject[] gameUI;

    [Header("Local player buttons")]
    public Button merchantBtn;
    public Button warriorBtn;
    public Button wandererBtn;
    public Button readyBtn;
    public TMP_Text myStatusText;
    public TMP_Text opponentStatusText;

    private PlayerClass localClass = PlayerClass.Merchant;
    private readonly Dictionary<int, int> confirmedClasses = new();

    void Awake() { Instance = this; }

    void Start()
    {
        selectionPanel.SetActive(true);
        choiceManagerObject?.SetActive(false);
        foreach (var go in gameUI) if (go != null) go.SetActive(false);

        merchantBtn?.onClick.AddListener(() => SelectClass(PlayerClass.Merchant));
        warriorBtn?.onClick.AddListener(() => SelectClass(PlayerClass.Warrior));
        wandererBtn?.onClick.AddListener(() => SelectClass(PlayerClass.Wanderer));
        readyBtn?.onClick.AddListener(SetReady);

        if (legendBtn != null && classInfoPanel != null)
        {
            classInfoPanel.SetActive(false);
            legendBtn.onClick.AddListener(() => classInfoPanel.SetActive(!classInfoPanel.activeSelf));
            classInfoPanel.transform.Find("CloseBtn")?.GetComponent<Button>()
                ?.onClick.AddListener(() => classInfoPanel.SetActive(false));
        }

        HighlightClass(localClass);
        if (myStatusText != null) myStatusText.text = "Pick your class";
        if (opponentStatusText != null) opponentStatusText.text = "Waiting for opponent...";
    }

    void SelectClass(PlayerClass cls)
    {
        localClass = cls;
        HighlightClass(cls);
    }

    void SetReady()
    {
        if (readyBtn != null) readyBtn.interactable = false;
        if (myStatusText != null) myStatusText.text = "Ready!";
        photonView.RPC(nameof(RPC_SetClass), RpcTarget.All,
            PhotonNetwork.LocalPlayer.ActorNumber, (int)localClass);
    }

    [PunRPC]
    void RPC_SetClass(int actor, int classIndex)
    {
        confirmedClasses[actor] = classIndex;

        bool isMe = actor == PhotonNetwork.LocalPlayer.ActorNumber;
        if (!isMe && opponentStatusText != null) opponentStatusText.text = "Opponent ready!";

        if (!PhotonNetwork.IsMasterClient) return;
        if (confirmedClasses.Count < PhotonNetwork.PlayerList.Length) return;

        // Apply class to each player and start game
        foreach (var kv in confirmedClasses)
        {
            int actorNum = kv.Key;
            PlayerClass cls = (PlayerClass)kv.Value;

            if (!PlayerMoverOn.MoversByActor.ContainsKey(actorNum)) continue;
            PlayerMoverOn mover = PlayerMoverOn.MoversByActor[actorNum];

            mover.photonView.RPC(nameof(PlayerMoverOn.RPC_SetPlayerClass), RpcTarget.All, kv.Value);

            int startGold = ClassData.StartingGold(cls);
            if (startGold > 0)
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All,
                    (int)InventoryOn.ItemType.Gold, startGold);

            if (ClassData.StartsWithWeapons(cls))
            {
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Sword, 1);
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Shield, 1);
                mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)InventoryOn.ItemType.Bow, 1);
            }
        }

        photonView.RPC(nameof(RPC_StartGame), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        selectionPanel.SetActive(false);
        classInfoPanel?.SetActive(false);
        legendBtn?.gameObject.SetActive(false);
        choiceManagerObject?.SetActive(true);
        foreach (var go in gameUI) if (go != null) go.SetActive(true);
        TurnGameManagerOn.Instance?.MasterStartGame();
    }

    void HighlightClass(PlayerClass selected)
    {
        SetBtnHighlight(merchantBtn, selected == PlayerClass.Merchant);
        SetBtnHighlight(warriorBtn,  selected == PlayerClass.Warrior);
        SetBtnHighlight(wandererBtn, selected == PlayerClass.Wanderer);
    }

    void SetBtnHighlight(Button btn, bool on)
    {
        if (btn == null) return;
        Transform outline = btn.transform.Find("Color/Yellow/Outline");
        if (outline == null) return;
        Image img = outline.GetComponent<Image>();
        if (img != null)
            img.color = on ? new Color(1f, 0.15f, 0.15f, 1f) : new Color(1f, 0.87f, 0f, 0.35f);
    }
}
