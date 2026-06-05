using UnityEngine;
using UnityEngine.EventSystems;

// Attach to Red/Blue/Yellow choice buttons — highlights reachable cities based on simulated position
public class ColorButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string color; // "Red", "Blue", or "Yellow"

    // Auto-detected at Start: which action slot (0-3) this button belongs to
    private int groupIndex = 0;

    void Start()
    {
        // Walk up the hierarchy to find the ChoiceGroup ancestor,
        // then find its index in ChoiceManager.choiceGroups
        if (ChoiceManager.Instance?.choiceGroups == null) return;

        Transform t = transform.parent;
        while (t != null)
        {
            var cg = t.GetComponent<ChoiceGroup>();
            if (cg != null)
            {
                for (int i = 0; i < ChoiceManager.Instance.choiceGroups.Length; i++)
                {
                    if (ChoiceManager.Instance.choiceGroups[i] == cg)
                    {
                        groupIndex = i;
                        return;
                    }
                }
                break;
            }
            t = t.parent;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance?.players == null) return;
        int idx    = GameManager.Instance.currentPlayerIndex;
        var player = GameManager.Instance.players[idx];
        if (player == null) return;

        string simulatedCity = SimulatePath(player);
        PathHighlighter.Instance?.ShowFor(simulatedCity, color);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PathHighlighter.Instance?.HideAll();
    }

    // Walk through all color moves selected in groups 0..groupIndex-1
    // to figure out where the player will actually be at this action slot
    string SimulatePath(PlayerMover player)
    {
        string current = player.currentCityName;
        string prev    = null;

        var groups = ChoiceManager.Instance?.choiceGroups;
        if (groups == null) return current;

        for (int i = 0; i < groupIndex && i < groups.Length; i++)
        {
            string selected  = groups[i].GetSelectedName();
            string moveColor = ButtonToColor(selected);
            if (moveColor == null) continue; // Yes/No or nothing — no movement

            string next = FindNext(current, prev, moveColor);
            if (next != null)
            {
                prev    = current;
                current = next;
            }
        }

        return current;
    }

    // Mirrors PlayerMover.FindNextCityByColor: prefers not going back to prev city
    string FindNext(string fromCity, string prevCity, string moveColor)
    {
        var connections = PathFinder.Instance?.GetConnections(fromCity, moveColor);
        if (connections == null || connections.Count == 0) return null;

        foreach (string city in connections)
            if (city != prevCity) return city;

        return connections[0]; // only option leads back — use it anyway
    }

    string ButtonToColor(string btn) => btn switch
    {
        "RedB"    => "Red",
        "BlueB"   => "Blue",
        "YellowB" => "Yellow",
        _         => null
    };
}
