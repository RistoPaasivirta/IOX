using System;
using UnityEngine;

public class FovBlocker : MonoBehaviour
{
    Func<Vector3> startFunc;
    Func<Vector3> endFunc;
    public Vector3 start { get { return startFunc(); } }
    public Vector3 end { get { return endFunc(); } }

    public float Dot(Vector3 position) => 
        Vector3.Dot(transform.up, (position - transform.position).normalized);

    void Start ()
    {
        if (gameObject.isStatic)
        {
            Vector3 precalculatedStart = transform.position + transform.right * transform.localScale.x / 2;
            startFunc = () => { return precalculatedStart; };

            Vector3 precalculatedEnd = transform.position - transform.right * transform.localScale.x / 2;
            endFunc = () => { return precalculatedEnd; };
        }
        else
        {
            startFunc = () => { return transform.position + transform.right * transform.localScale.x / 2; };
            endFunc = () => { return transform.position - transform.right * transform.localScale.x / 2; };
        }
	}
}
