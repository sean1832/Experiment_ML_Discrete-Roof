using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProMeshConstruct : MonoBehaviour
{


    public static void ConstructMesh(List<Vector3> vertices, bool reverseTriangle, GameObject parent = null)
    {
        int[] triangles = ProMeshConstruct.GetTriangleVertices(vertices, 1, reverseTriangle);

        // create new mesh
        Mesh mesh = new Mesh();
        // create a new gameObject
        GameObject roofSub = new GameObject("roofSub");
        // parent to nest into
        if (parent != null)
        {
            roofSub.transform.parent = parent.transform;
        }

        roofSub.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        roofSub.AddComponent<MeshFilter>().mesh = mesh;

        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }



    private static int[] GetTriangleVertices(List<Vector3> vertices, int columnNum, bool reverseTriangle)
    {
        int vertNum = vertices.Count;
        int quadNum = ((vertNum - columnNum) / (columnNum + 1)) * columnNum;
        int triVertNum = quadNum * 6;

        int[] triangle = new int[triVertNum];
        int vert = 0;
        int tri = 0;

        int[] triangleOrder = new int[] { 0, 2, 1, 2, 3, 1 };
        int[] reverseTriOrder = new int[] { 0, 1, 2, 1, 3, 2 };

        for (int i = 0; i < quadNum; i++)
        {
            int[] order;
            order = !reverseTriangle ? triangleOrder : reverseTriOrder;

            triangle[tri] = vert + order[0];
            triangle[tri + 1] = vert + order[1];
            triangle[tri + 2] = vert + order[2];

            triangle[tri + 3] = vert + order[3];
            triangle[tri + 4] = vert + order[4];
            triangle[tri + 5] = vert + order[5];

            vert += 2;
            tri += 6;
        }

        return triangle;
    }
}
