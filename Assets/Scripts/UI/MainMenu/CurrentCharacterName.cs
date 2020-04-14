using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CurrentCharacterName : MonoBehaviour
{
    [SerializeField] private Mode CurrentMode = Mode.RPGName;

    Text text; 

    public enum Mode
    {
        RPGName,
        LevelAndProfession
    }

    private void Awake() =>
        text = GetComponent<Text>();

    private void OnEnable()
    {
        if (CurrentMode == Mode.RPGName)
            text.text = PlayerInfo.CurrentLocal.RPGName;
        else
            text.text = "Level " + PlayerInfo.CurrentLocal.Level + " " + PlayerInfo.CurrentLocal.RPGProfession;
    }
}
