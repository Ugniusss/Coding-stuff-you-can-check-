using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManagerOn : MonoBehaviour
{
    public ChoiceGroup[] choiceGroups;
    public Button doneButton;

    void Start()
    {
        doneButton.onClick.RemoveAllListeners();
        doneButton.onClick.AddListener(OnDonePressed);
    }

    void Update()
    {
        if (TurnGameManagerOn.Instance == null) return;
        doneButton.interactable =
            PhotonNetwork.LocalPlayer.ActorNumber == TurnGameManagerOn.Instance.currentTurnActor
            && !TeleportUI.IsPicking;
    }

    void OnDonePressed()
    {
        // Collect move choices
        string[] moves = new string[choiceGroups.Length];
        for (int i = 0; i < choiceGroups.Length; i++)
        {
            moves[i] = choiceGroups[i].GetSelectedName();
            if (string.IsNullOrEmpty(moves[i])) return;
        }

        // Build 5-element array: [abilityStr, m0, m1, m2, m3]
        string abilityStr = AbilityUIOn.Instance != null
            ? AbilityUIOn.Instance.ConsumePendingAbility()
            : "None";

        string[] choices = new string[moves.Length + 1];
        choices[0] = abilityStr;
        for (int i = 0; i < moves.Length; i++) choices[i + 1] = moves[i];

        TurnGameManagerOn.Instance.SubmitChoices(choices);

        foreach (var g in choiceGroups) g.ResetSelection();
        AbilityUIOn.Instance?.Hide();
    }
}
