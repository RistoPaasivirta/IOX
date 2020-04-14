using UnityEngine;
using UnityEngine.Events;

public class CommonUsableObject : MonoBehaviour, UsableObject
{
    [SerializeField] private string ShowText = "Usable Object";
    [SerializeField] private Color TextColorFar = Color.grey;
    [SerializeField] private Color TextColorClose = Color.white;
    [SerializeField] private int ShowTextDistance = 6;
    [SerializeField] private float UseDistance = 2;
    [SerializeField] private Vector3 Offset = Vector3.zero;

    public bool AllowOnlyPlayer = true;
    public UnityEvent<MonsterCharacter> OnUse = new UnityEventMonster();
    public UnityEvent AfterUse = new UnityEvent();

    FloatingTexts.FloatingText floatingText = null;
    Vec2I lastPlayerGridPosition;
    Vec2I gridPos;

    public void Use(MonsterCharacter caller)
    {
        if (AllowOnlyPlayer)
            if (caller.GetComponent<PlayerControls>() == null)
                return;

        if (Vec2I.Max(TheGrid.GridPosition(caller.transform.position), TheGrid.GridPosition(transform.position)) > UseDistance)
            return;

        OnUse.Invoke(caller);
        AfterUse.Invoke();
    }

    private void Awake()
    {
        Messaging.Player.Position.AddListener((v) => 
        {
            Vec2I g = TheGrid.GridPosition(v);
            
            if (lastPlayerGridPosition == g)
                return;

            lastPlayerGridPosition = g;

            int gridDistance = Vec2I.Max(g, TheGrid.GridPosition(transform.position));

            if (gridDistance <= ShowTextDistance)
            {
                if (floatingText == null)
                    floatingText = Messaging.GUI.FloatingText(transform.position + Offset, ShowText, TextColorFar);
                
                floatingText.color = gridDistance <= UseDistance ? TextColorClose : TextColorFar;
            }
            else if (floatingText != null && gridDistance > ShowTextDistance)
            {
                Destroy(floatingText.gameObject);
                floatingText = null;
            }
        });
    }

    private void OnDisable()
    {
        if (floatingText != null)
        {
            Destroy(floatingText.gameObject);
            floatingText = null;
        }
    }
}
