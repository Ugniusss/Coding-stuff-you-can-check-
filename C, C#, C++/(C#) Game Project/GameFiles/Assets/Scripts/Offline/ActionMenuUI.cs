using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ActionMenuUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text titleText;

    public Button yesButton;
    public Button noButton;

    public Button redButton;
    public Button blueButton;
    public Button yellowButton;

    public Button confirmButton;

    private bool selectedInteraction = false;
    private bool interactionChosen = false;

    private PlayerAction.PathColor? selectedColor = null;

    private int currentStep = 0;
    private const int maxSteps = 4;
    private int currentPlayerIndex;
    private List<PlayerAction> plannedActions = new List<PlayerAction>();

    private void Start()
    {
        yesButton.onClick.AddListener(() => SelectInteraction(true));
        noButton.onClick.AddListener(() => SelectInteraction(false));

        redButton.onClick.AddListener(() => SelectColor(PlayerAction.PathColor.Red));
        blueButton.onClick.AddListener(() => SelectColor(PlayerAction.PathColor.Blue));
        yellowButton.onClick.AddListener(() => SelectColor(PlayerAction.PathColor.Yellow));

        confirmButton.onClick.AddListener(OnConfirm);
        //Hide();
    }

    public void Show(int playerIndex)
    {
        currentPlayerIndex = playerIndex;
        panel.SetActive(true);
        currentStep = 0;
        plannedActions.Clear();
        titleText.text = $"Player {playerIndex + 1} - Action {currentStep + 1}/4";
        ResetSelection();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void SelectInteraction(bool value)
    {
        selectedInteraction = value;
        interactionChosen = true;
        //Debug.Log("Interaction: " + value);
    }

    void SelectColor(PlayerAction.PathColor color)
    {
        selectedColor = color;
        //Debug.Log("Color: " + color);
    }

    void OnConfirm()
    {
        if (!interactionChosen || selectedColor == null)
        {
            //Debug.LogWarning("Both interaction and color must be selected");
            return;
        }

        plannedActions.Add(new PlayerAction(selectedColor.Value, selectedInteraction));
        currentStep++;

        if (currentStep >= maxSteps)
        {
            GameManager.Instance.ReceivePlannedActions(currentPlayerIndex, plannedActions.ToArray());
           // Hide();
        }
        else
        {
            titleText.text = $"Player {currentPlayerIndex + 1} - Action {currentStep + 1}/4"; //Gal reikes
            ResetSelection();
        }
    }

    void ResetSelection()
    {
        interactionChosen = false;
        selectedColor = null;
    }
}
