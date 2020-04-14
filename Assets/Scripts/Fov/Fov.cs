using System.Collections.Generic;
using UnityEngine;

public class Fov : MonoBehaviour
{
    List<FovBlocker> activeBlockers = new List<FovBlocker>(1024);
    [SerializeField] private GameObject casterObject = null;

    MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh { name = "View Mesh" };
        viewMeshFilter = gameObject.GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FovBlocker blocker = collision.GetComponent<FovBlocker>();
        if (blocker != null)
            activeBlockers.Add(blocker);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FovBlocker blocker = collision.GetComponent<FovBlocker>();
        if (blocker != null)
            activeBlockers.Remove(blocker);
    }

    private void LateUpdate() 
    {
        viewMesh.Clear();
        if (activeBlockers.Count < 1)
            return;

        Vector3[] vertices = new Vector3[activeBlockers.Count * 4];
        int[] triangles = new int[activeBlockers.Count * 6];

        int i = 0; int t = 0;
        foreach (FovBlocker blocker in activeBlockers)
        {
            vertices[i++] = transform.InverseTransformPoint(blocker.start);
            vertices[i++] = transform.InverseTransformPoint(blocker.end);
            vertices[i++] = transform.InverseTransformPoint(blocker.start + (blocker.start - casterObject.transform.position).normalized * 40);
            vertices[i++] = transform.InverseTransformPoint(blocker.end + (blocker.end - casterObject.transform.position).normalized * 40);

            if (blocker.Dot(casterObject.transform.position) < 0)
            {
                triangles[t++] = i - 3;
                triangles[t++] = i - 2;
                triangles[t++] = i - 4;
                triangles[t++] = i - 1;
                triangles[t++] = i - 2;
                triangles[t++] = i - 3;
            }
            else
            {
                triangles[t++] = i - 4;
                triangles[t++] = i - 2;
                triangles[t++] = i - 3;
                triangles[t++] = i - 3;
                triangles[t++] = i - 2;
                triangles[t++] = i - 1;
            }
        }

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
}
