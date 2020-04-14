using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class ClassInputField : MonoBehaviour
{
    private void Awake()
    {
        InputField inputField = GetComponent<InputField>();

        Messaging.Player.RPGProfession.AddListener(() =>
        {
            inputField.text = PlayerInfo.CurrentLocal.RPGProfession;
        });

        inputField.onEndEdit.AddListener((s) =>
        {
            PlayerInfo.CurrentLocal.RPGProfession = s;
            Messaging.Player.RPGProfession.Invoke();
        });

        inputField.text = PlayerInfo.CurrentLocal.RPGProfession;
    }
}
