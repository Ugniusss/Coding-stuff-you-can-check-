using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public ChoiceManager choiceManager;

    void Start()
    {
        //Debug.Log("GameFlow started");

        if (choiceManager != null)
        {
            choiceManager.gameObject.SetActive(true);  
        }
        else
        {
            //Debug.LogWarning("ChoiceManager is not assigned in GameFlow");
        }
    }
}
