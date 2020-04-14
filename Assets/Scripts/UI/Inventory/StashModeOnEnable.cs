using UnityEngine;

public class StashModeOnEnable : MonoBehaviour
{
    public enum Side
    {
        Left,
        Right
    }

    [SerializeField] private Side ScreenSide = Side.Left;
    [SerializeField] private Stash.StashMode StashMode = Stash.StashMode.Disabled;

    private void OnEnable()
    {
        if (ScreenSide == Side.Left)
            Stash.LeftStashMode = StashMode;
        else
            Stash.RightStashMode = StashMode;
    }
}
