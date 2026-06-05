using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLog : MonoBehaviour
{
    public static GameLog Instance;

    public Transform entryContainer;
    public TMP_Text entryPrefab;
    public int maxEntries = 6;

    private readonly Queue<TMP_Text> entries = new();

    void Awake() => Instance = this;

    public void Add(string message)
    {
        TMP_Text entry = Instantiate(entryPrefab, entryContainer);
        entry.gameObject.SetActive(true);
        if (message.StartsWith("P1") || message.StartsWith("Player 1"))      entry.color = new Color(1f, 0.3f, 0.3f);
        else if (message.StartsWith("P2") || message.StartsWith("Player 2")) entry.color = new Color(0.35f, 0.55f, 1f);
        entry.text = message;

        entries.Enqueue(entry);
        while (entries.Count > maxEntries)
            Destroy(entries.Dequeue().gameObject);
    }
}
