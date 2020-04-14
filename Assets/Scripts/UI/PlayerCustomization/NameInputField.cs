using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class NameInputField : MonoBehaviour
{
    private void Awake()
    {
        InputField inputField = GetComponent<InputField>();

        Messaging.Player.RPGName.AddListener(() => 
        {
            inputField.text = PlayerInfo.CurrentLocal.RPGName;
        });

        inputField.onEndEdit.AddListener((s) => 
        {
            PlayerInfo.CurrentLocal.RPGName = s;
            Messaging.Player.RPGName.Invoke();
        });

        inputField.text = PlayerInfo.CurrentLocal.RPGName;
    }
}
