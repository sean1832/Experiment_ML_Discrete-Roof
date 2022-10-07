using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedualMesh : MonoBehaviour
{
    public static List<Vector3> GetTopVertices(List<Vector3> vertices, GameObject ceiling)
    {
        List<Vector3> hitVertices = new List<Vector3>();

        foreach (Vector3 vertex in vertices)
        {
            var hitInfo = new RaycastHit();
            Ray ray = new Ray(vertex, Vector3.up);
            if (!Physics.Raycast(ray, out hitInfo, 20f)) continue;
            if (hitInfo.collider.gameObject == ceiling)
            {
                hitVertices.Add(vertex);
            }
        }
        return hitVertices;
    }
}
