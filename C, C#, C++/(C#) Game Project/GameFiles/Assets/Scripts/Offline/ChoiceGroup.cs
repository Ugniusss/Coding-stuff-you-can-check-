using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceGroup : MonoBehaviour
{
    public Button[] buttons;
    private Button selectedButton;

    // Cache original sprites per button so we can restore them
    private Dictionary<Button, Sprite>      origNormal      = new();
    private Dictionary<Button, Sprite>      origHighlighted = new();
    private Dictionary<Button, Sprite>      origSelected    = new();

    void Start()
    {
        foreach (var btn in buttons)
        {
            origNormal[btn]      = btn.image.sprite;
            origHighlighted[btn] = btn.spriteState.highlightedSprite;
            origSelected[btn]    = btn.spriteState.selectedSprite;
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }
    }

    void OnButtonClicked(Button clicked)
    {
        if (selectedButton != null)
            ApplyVisual(selectedButton, false);

        selectedButton = clicked;
        ApplyVisual(clicked, true);
    }

    void ApplyVisual(Button btn, bool selected)
    {
        Sprite pressedSprite = btn.spriteState.pressedSprite;

        if (selected && pressedSprite != null)
        {
            // Force the displayed sprite AND all transition states to pressed sprite
            // so clicking elsewhere or hovering can't revert it
            btn.image.sprite = pressedSprite;

            var ss = btn.spriteState;
            ss.highlightedSprite = pressedSprite;
            ss.selectedSprite    = pressedSprite;
            btn.spriteState = ss;
        }
        else
        {
            // Restore original sprites
            btn.image.sprite = origNormal.TryGetValue(btn, out var n) ? n : btn.image.sprite;

            var ss = btn.spriteState;
            ss.highlightedSprite = origHighlighted.TryGetValue(btn, out var h) ? h : null;
            ss.selectedSprite    = origSelected.TryGetValue(btn, out var s)    ? s : null;
            btn.spriteState = ss;
        }
    }

    public string GetSelectedName()
    {
        Debug.Log($"[ChoiceGroup] Selected button: {selectedButton?.name}");
        return selectedButton ? selectedButton.name : "";
    }

    public void ResetSelection()
    {
        if (selectedButton != null)
        {
            ApplyVisual(selectedButton, false);
            selectedButton = null;
        }
    }
}
