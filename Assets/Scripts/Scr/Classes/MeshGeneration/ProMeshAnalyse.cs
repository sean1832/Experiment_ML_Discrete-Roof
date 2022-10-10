using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProMeshAnalyse : MonoBehaviour
{
    public static float GetMeshSurfaceArea(Mesh mesh)
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        float sum = 0.0f;

        for (int i = 0; i < triangles.Length; i +=3)
        {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a,b).magnitude;
        }

        return sum;
    }
}
