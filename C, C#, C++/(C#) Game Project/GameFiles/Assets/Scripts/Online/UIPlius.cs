using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIPlius : MonoBehaviour
{
    public static UIPlius Instance;

    [Header("Turn Counter")]
    public TMP_Text turnCounterText;

    [Header("Turn Images")]
    public GameObject turnImage1;
    public GameObject turnImage2;

    [Header("Action Menu")]
    public CanvasGroup actionMenuGroup;

    [Header("Win Images")]
    public GameObject player1WinImage;
    public GameObject player2WinImage;

    void Awake()
    {
        Instance = this;
    }

    public void SetTurn(int actor)
    {
        bool isMyTurn = PhotonNetwork.LocalPlayer.ActorNumber == actor;

        Debug.Log($"[UI] SetTurn actor={actor}, isMyTurn={isMyTurn}");

        turnImage1.SetActive(actor == 1);
        turnImage2.SetActive(actor == 2);

        actionMenuGroup.alpha = isMyTurn ? 1f : 0.4f;
        actionMenuGroup.interactable = isMyTurn;
        actionMenuGroup.blocksRaycasts = isMyTurn;
    }

    public void SetTurnCounter(int value)
    {
        turnCounterText.text = value + "/15";
    }

    public void ShowWin(int actor)
    {
        player1WinImage.SetActive(actor == 1);
        player2WinImage.SetActive(actor == 2);
    }
}