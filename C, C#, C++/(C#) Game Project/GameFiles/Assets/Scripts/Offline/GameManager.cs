using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerMover[] players;
    private bool gameEnded = false;

    public Image player1WinImage;
    public Image player2WinImage;

    public Image player1TurnImage;
    public Image player2TurnImage;
    public int currentPlayerIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void ReceivePlannedActions(int playerIndex, PlayerAction[] actions)
    {
       
       //Debug.Log($"Received actions from Player {playerIndex + 1}");
        foreach (var action in actions)
        {
            //Debug.Log($" - Action: {action.moveColor}, Interact: {action.doAction}");
        }
    }

    public TMP_Text turnText;
    private int turnNumber = 1;
    public int TurnNumber => turnNumber;

    private void Start()
    {
        UpdateTurnText();
    }

    public void AdvanceTurn()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.turnChange);
        if (gameEnded) return;
        turnNumber++;
        UpdateTurnText();
        ShowTurnImage();
        MobObjectSpawner.Instance?.CheckShrineRespawns();
        

        if (turnNumber >= 15)
        {
            gameEnded = true;
            //Debug.Log("Turn 15 reached.  winner...");
            EvaluateWinner();
        }
    }

    private void UpdateTurnText()
    {
        if (turnText != null)
        {
            if (turnNumber > 15) { turnNumber = 15; } // Cap at 15
            turnText.text = $"{turnNumber}/15";
            //Debug.Log($"Turn updated to {turnNumber}");
        }
        else
        {
            //Debug.LogWarning("turnText not assigned in GameManager.");
        }
    }
    void EvaluateWinner()
    {
        Inventory inv1 = players[0].GetComponent<Inventory>();
        Inventory inv2 = players[1].GetComponent<Inventory>();

        int gold1 = inv1.GetItemCount(Inventory.ItemType.Gold);
        int gold2 = inv2.GetItemCount(Inventory.ItemType.Gold);

        //Debug.Log($"Gold P1: {gold1}, P2: {gold2}");

        if (gold1 > gold2)
        {
            ShowWinImage(1);
        }
        else if (gold2 > gold1)
        {
            ShowWinImage(2);
        }
        else
        {
            // Tie on gold check total items
            int items1 = inv1.GetItemCount(Inventory.ItemType.Sword)
                       + inv1.GetItemCount(Inventory.ItemType.Shield)
                       + inv1.GetItemCount(Inventory.ItemType.Bow);

            int items2 = inv2.GetItemCount(Inventory.ItemType.Sword)
                       + inv2.GetItemCount(Inventory.ItemType.Shield)
                       + inv2.GetItemCount(Inventory.ItemType.Bow);

            //Debug.Log($"Gold tie → comparing items: P1: {items1}, P2: {items2}");

            if (items1 > items2)
                ShowWinImage(1);
            else if (items2 > items1)
                ShowWinImage(2);
            else
                Debug.Log("It's a full tie!"); // retai kada bus bet maybe
        }
    }

    void ShowWinImage(int playerIndex)
    {
        if (playerIndex == 1 && player1WinImage != null)
            player1WinImage.gameObject.SetActive(true);
        else if (playerIndex == 2 && player2WinImage != null)
            player2WinImage.gameObject.SetActive(true);

        Debug.Log($"Player {playerIndex} wins!");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.win);

        StartCoroutine(ReturnToMainMenuAfterDelay());
    }
    IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainMenu"); 
    }

    public void ShowTurnImage()
    {

        if (player1TurnImage != null) player1TurnImage.gameObject.SetActive(false);
        if (player2TurnImage != null) player2TurnImage.gameObject.SetActive(false);

        if (currentPlayerIndex == 0)
            player1TurnImage?.gameObject.SetActive(true);
        else
            player2TurnImage?.gameObject.SetActive(true);


        if (AudioManager.Instance != null && AudioManager.Instance.turnChange != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.turnChange);
        }



    }



}
