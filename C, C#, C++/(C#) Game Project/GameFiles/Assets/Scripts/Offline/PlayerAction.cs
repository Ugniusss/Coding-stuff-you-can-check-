using UnityEngine;

[System.Serializable]
public class PlayerAction
{
    public enum PathColor { Red, Blue, Yellow }

    public PathColor? moveColor;  
    public bool doAction;

    public PlayerAction(PathColor? color, bool action)
    {
        moveColor = color;
        doAction = action;
    }
}
