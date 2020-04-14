using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CurrentCharacterSlotName : MonoBehaviour
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
        if (SaveLoadSystem.CurrentPlayerSlot < 0 || SaveLoadSystem.CurrentPlayerSlot >= PlayerInfoHolder.LoadedHolders.Count)
            return;

        PlayerInfoHolder i = PlayerInfoHolder.LoadedHolders[SaveLoadSystem.CurrentPlayerSlot];

        if (CurrentMode == Mode.RPGName)
            text.text = i.RPGName;
        else
            text.text = "Level " + i.Level + " " + i.RPGProfession;
    }
}
