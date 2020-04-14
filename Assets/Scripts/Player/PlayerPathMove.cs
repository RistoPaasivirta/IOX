using UnityEngine;

[RequireComponent(typeof(MonsterCharacter), typeof(ThingController))]
public class PlayerPathMove : MonoBehaviour
{
    MonsterCharacter character;
    ThingController thing;

    private void Awake()
    {
        character = GetComponent<MonsterCharacter>();
        thing = GetComponent<ThingController>();
        character.OnInputRequest = FillInputFrame;
    }

    float timer = 0f;
    [SerializeField] private float DecisionTime = .35f;

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    Vector2 dir = Vector2.zero;
    Vec2I lastMouseGrid;

    private void FillInputFrame(ref MonsterCharacter.InputFrame frame)
    {
        frame.up = dir.y;
        frame.right = dir.x;
        frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (timer > 0f)
            return;

        timer = DecisionTime;
        dir = Vector2.zero;

        Mouse.UpdateWorldPosition();

        if (Mouse.InWorld)
        {
            if (Mouse.GridPosition != lastMouseGrid)
            {
                Vec2I a = thing.gridPos;
                Vec2I b = Mouse.GridPosition;

                if (AI.GetPath(a, b, Vec2I.Max(a, b), out Vec2I[] path))
                {
                    AI.NaturalizePath(ref path, 10);
                    if (path.Length > 1)
                        dir = (Vector2)(path[1] - thing.gridPos);
                    else
                        dir = ((Vector2)Mouse.WorldPosition - (Vector2)transform.position).ZeroNormalize();
                }
                else
                    dir = ((Vector2)Mouse.WorldPosition - (Vector2)transform.position).ZeroNormalize();

                lastMouseGrid = Mouse.GridPosition;
            }
        }
    }
}
