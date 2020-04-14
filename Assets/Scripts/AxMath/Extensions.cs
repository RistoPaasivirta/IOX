using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 DropY(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3 DropZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector3 DropXZ(this Vector3 vector)
    {
        return new Vector3(0, vector.y, 0);
    }

    public static Vector2 SafeNormalize(this Vector2 v)
    {
        if (v == Vector2.zero)
            return Vector2.right;

        return v.normalized;
    }

    public static Vector2 ZeroNormalize(this Vector2 v)
    {
        if (v == Vector2.zero)
            return Vector2.zero;

        return v.normalized;
    }

    public static Vector2 SafeNormalize(this Vector2 v, out float distance)
    {
        if (v == Vector2.zero)
        {
            distance = 0f;
            return Vector2.right;
        }

        distance = v.magnitude;
        return new Vector2(v.x / distance, v.y / distance);
    }

    public static Vector2 RotateCCW(this Vector2 v) =>
        new Vector2(-v.y, v.x);

    public static Vector2 RotateCW(this Vector2 v) =>
        new Vector2(v.y, -v.x);

    public static List<Transform> GetChildsRecursive(this Transform trm)
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform t in trm)
        {
            list.Add(t);
            list.AddRange(t.GetChildsRecursive());
        }

        return list;
    }

    public static void SetLayerRecursively(this GameObject target, int layer)
    {
        target.layer = layer;

        foreach (Transform child in target.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}