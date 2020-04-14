using UnityEngine;

public static class Mouse
{
    public static float SelectionRadius = .6f;

    public static Vector3 WorldPosition { get; private set; }
    public static Vec2I GridPosition { get; private set; }

    public static bool OnScreen { get; private set; }
    public static bool InWorld { get; private set; }

    public static ThingController Target { get; private set; }
    static readonly Plane worldPlane = new Plane(new Vector3(0,0,-1), 0);

    public static void UpdateWorldPosition()
    {
        InWorld = false;

        Rect screen = new Rect(0, 0, Screen.width, Screen.height);
        OnScreen = screen.Contains(Input.mousePosition);
        if (!OnScreen) return;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(mouseRay, SelectionRadius, 300f, LayerMask.Shootables, QueryTriggerInteraction.Ignore);

        ThingController thing = null;
        float closest = float.MaxValue;
        Vector3 hitpoint = Vector3.zero;
        Vector3 hitnormal = Vector3.zero;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<PlayerControls>() != null)
                continue;

            ThingController t = hit.collider.GetComponent<ThingController>();
            if (t == null)
                continue;

            Vector3 origin = Camera.main.transform.position;

            float m = (hit.point - origin).sqrMagnitude;
            if (m < closest)
            {
                hitpoint = hit.point;
                hitnormal = hit.normal;
                closest = m;
                thing = t;
            }
        }

        if (Target != null)
            if (Target != thing)
                Target.OnMouseLeave.Invoke();

        Target = thing;

        if (Target != null)
        {
            Target.OnMouseEnter.Invoke();

            WorldPosition = hitpoint;
            InWorld = true;
        }
        else
        {
            //we didn't hit a thing, get point on virtual plane

            if (worldPlane.Raycast(mouseRay, out float distance))
            {
                WorldPosition = mouseRay.origin + mouseRay.direction * distance;
                InWorld = true;
            }
        }

        if (InWorld)
            GridPosition = TheGrid.GridPosition(WorldPosition);
    }
}
