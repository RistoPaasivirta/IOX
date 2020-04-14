using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MonsterCharacter), typeof(ThingController))]
public abstract class Behavior : MonoBehaviour
{
    [SerializeField] private Vector2 ExtraScreenBuffer = Vector2.zero;
    [SerializeField] private bool NeverLoseTarget = false;
    [SerializeField] private float RayDistance = 30f;
    [SerializeField] private float RefreshTargetTime = 1f;

    protected System.Func<MonsterCharacter> GetNewTarget = () => { return null; };
    protected MonsterCharacter target;
    protected MonsterCharacter character;
    protected ThingController thing;
    protected virtual void OnAwake() { }
    protected virtual void OnUpdate() { }

    public virtual void Restart() { }
    public UnityEvent OnLoseTarget = new UnityEvent();
    public UnityEvent OnGainTarget = new UnityEvent();

    float onScreenLingerTimer = 0f;
    float refreshTargetTimer;

    private void Awake()
    {
        character = GetComponent<MonsterCharacter>();
        thing = GetComponent<ThingController>();

        refreshTargetTimer = Random.value;

        GetNewTarget = DefaultGetNewTarget;

        OnAwake();
    }

    private void Update()
    {
        if (NeverLoseTarget && target != null)
        {
            OnUpdate();
            return;
        }

        if (onScreenLingerTimer > 0f)
            onScreenLingerTimer -= Time.deltaTime;

        refreshTargetTimer -= Time.deltaTime;

        if (refreshTargetTimer <= 0)
        {
            refreshTargetTimer = RefreshTargetTime;

            if (target == null)
            {
                target = GetNewTarget.Invoke();

                if (target != null)
                    OnGainTarget.Invoke();
            }
            else
            {
                target = GetNewTarget.Invoke();

                if (target == null)
                    OnLoseTarget.Invoke();
            }
        }

        OnUpdate();
    }

    public Quaternion TowardsTarget
    {
        get
        {
            Vector3 dif = (target.transform.position - transform.position).DropZ();
            return Quaternion.Euler(0, 0, AxMath.SafeAtan2(dif.y, dif.x) * Mathf.Rad2Deg);
        }
    }

    public Quaternion TowardsPosition(Vector3 position)
    {
        Vector2 dif = (position - transform.position);
        return Quaternion.Euler(0, 0, AxMath.SafeAtan2(dif.y, dif.x) * Mathf.Rad2Deg);
    }

    public Vector3 TargetPredictedPosition(float bulletSpeed, float amount)
    {
        float distance = (target.transform.position - transform.position).magnitude;
        Vector3 prediction = (Vector3)target.physicsBody.velocity * (distance / bulletSpeed);
        return target.transform.position + prediction * amount;
    }

    protected float TrueDistanceToTarget
    {
        get
        {
            return (transform.position - target.transform.position).magnitude;
        }
    }

    protected MonsterCharacter DefaultGetNewTarget()
    {
        //use this to only allow target acquisition when monster is near screen
        /*Vector2 distance = transform.position - Camera.main.transform.position;
        if (Mathf.Max(distance.x) > Options.AlertDistance.x || Mathf.Max(distance.y) > Options.AlertDistance.y)
            return null;*/

        Vector2 size = Options.AlertDistance;

        Collider2D[] hits = Physics2D.OverlapAreaAll(
            (Vector2)transform.position + size,
            (Vector2)transform.position - size,
            LayerMask.Walkers);

        float closest = float.MaxValue;
        MonsterCharacter m = null;

        foreach(Collider2D hit in hits)
        {
            MonsterCharacter monster = hit.GetComponent<MonsterCharacter>();

            if (monster == null)
                continue;

            if (monster == character)
                continue;

            if (character.Faction != Factions.AgainstAll)
                if (character.Faction == monster.Faction)
                    continue;

            float d = (monster.transform.position - transform.position).sqrMagnitude;
            if (d < closest)
            {
                m = monster;
                closest = d;
            }
        }

        return m;
    }

    protected Vec2I SmartMove
    {
        get
        {
            if (target == null)
                return Vec2I.zero;

            Vec2I a = thing.gridPos;
            Vec2I b = target.thingController.gridPos;

            if (!AI.GetPath(a, b, Vec2I.Max(a, b) * 2, out Vec2I[] path))
                return Vec2I.zero;

            if (path.Length > 1)
                return path[1] - thing.gridPos;

            //pathfinding failed... start wiggling, maybe it helps =D
            return Vec2I.directions[Random.Range(0, Vec2I.directions.Length)];
        }
    }

    protected bool CanSeeTargetGrid
    {
        get
        {
            return AI.CanSeeGridRay(thing.gridPos, target.thingController.gridPos);
        }
    }

    protected bool RayToTarget(Vector2 from)
    {
        Vector2 randomTargetPos = (Vector2)target.transform.position + Random.insideUnitCircle * target.TargetingRadius;

        //congratulations you won the lottery
        if (from == randomTargetPos)
            return true;

        RaycastHit2D[] hits = Physics2D.RaycastAll(from, (randomTargetPos - from).normalized, RayDistance, LayerMask.WallsWalkersAndShootables);

        float closest = float.MaxValue;
        Collider2D collider = null;
            
        foreach (RaycastHit2D hit in hits)
        {
            MonsterCharacter m = hit.collider.GetComponentInParent<MonsterCharacter>();
            if (m != null)
                if (m.Faction == character.Faction || m == character)
                    continue;

            float d = hit.distance;
            if (d < closest)
            {
                closest = d;
                collider = hit.collider;
            }
        }

        if (collider == null)
            return false;

        return collider.GetComponent<MonsterCharacter>() == target;
    }

    protected bool OnScreen
    {
        get
        {
            float x = Mathf.Abs(transform.position.x - Camera.main.transform.position.x);
            float y = Mathf.Abs(transform.position.y - Camera.main.transform.position.y);

            Vector2 size = Options.ScreenDistance + ExtraScreenBuffer;

            //this is to prevent players from abusing the AI by dancing in and out of distance
            if (x < size.x && y < size.y)
                onScreenLingerTimer = Options.OnScreenLingerTime;

            return onScreenLingerTimer > 0f;
        }
    }

    protected float AngleToTarget(Vector2 target)
    {
        Vector2 pos = transform.position;

        if (target == pos)
            return 0f;

        Vector2 toTarget = (target - pos).normalized;
        Vector2 forward = character.LookDirection * Vector3.right;

        return Vector2.Angle(forward, toTarget);
    }
}
