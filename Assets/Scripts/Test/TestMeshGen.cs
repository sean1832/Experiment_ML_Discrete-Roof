using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;
    [SerializeField] private GameObject _ceiling;
    [SerializeField] private GameObject _roofPointLayer;

    private List<Vector3> vertices;
    private List<Vector3> _hitVertices;
    private bool _isStarted = false;

    private Mesh mesh;


    // Start is called before the first frame update
    void Start()
    {
        vertices = Utilities.GetProjectedVertices(_resultMesh, "x", -5f, "Spawn", "checkPoint");
        Actions.GenerateGameObject(vertices, _roofPointLayer);
        _isStarted = true;
        StartCoroutine(WaitBeforeRayCase());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitBeforeRayCase()
    {
        yield return new WaitForSeconds(0.05f);
        _hitVertices = Utilities.GetTopVertices(vertices, _ceiling);

        List<Vector3> movedVertices = Utilities.OffsetVertices(_hitVertices, new Vector3(1, 0, 0), 20f);
        _hitVertices.AddRange(movedVertices);
        _hitVertices = _hitVertices.OrderBy(v => v.z).ToList();


        GameObject roofParent = new GameObject("roof");

        (Vector3[] triVertices, int[] triangles) = GenerateTriangle(_hitVertices, false);
        ConstructMesh(triVertices, triangles, roofParent);
        (Vector3[] triVerticesBack, int[] trianglesBack) = GenerateTriangle(_hitVertices, true);
        ConstructMesh(triVerticesBack, trianglesBack, roofParent);
    }

    private void OnDrawGizmos()
    {
        if (!_isStarted) return;
        if (_hitVertices == null) return;
        foreach (var vertex in _hitVertices)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(vertex, 0.2f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_hitVertices[2], 0.2f);
    }

    (Vector3[] triVertices, int[] triangles) GenerateTriangle(List<Vector3> vertices, bool reverseTriangle)
    {

        Vector3[] triangleVertices = vertices.ToArray();
        int[] triangle = GetTriangleVertices(vertices, 1, reverseTriangle);

        return (triangleVertices, triangle);

    }

    private static int[] GetTriangleVertices(List<Vector3> vertices, int columnNum, bool reverseTriangle)
    {
        int vertNum = vertices.Count;
        int quadNum = ((vertNum - columnNum) / (columnNum + 1)) * columnNum;
        int triVertNum = quadNum * 6;

        int[] triangle = new int[triVertNum];
        int vert = 0;
        int tri = 0;

        int[] triangleOrder = new int[]{0,2,1, 2,3,1};
        int[] reverseTriOrder = new int[]{0,1,2, 1,3,2};

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


    void ConstructMesh(Vector3[] vertices, int[] triangles, GameObject parent = null)
    {
        
        mesh = new Mesh();
        // create a new gameObject
        GameObject roofSub = new GameObject("roofSub");
        if (parent != null)
        {
            roofSub.transform.parent = parent.transform;
        }

        roofSub.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        roofSub.AddComponent<MeshFilter>().mesh = mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
}
