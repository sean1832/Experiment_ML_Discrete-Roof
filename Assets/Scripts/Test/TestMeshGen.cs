using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        vertices = Utilities.GetAllRoofVertices(_resultMesh, "Spawn", "checkPoint");
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

        List<Vector3> movedVertices = Utilities.MoveVertices(_hitVertices, new Vector3(1, 0, 0), 20f);
        _hitVertices.AddRange(movedVertices);
        _hitVertices = _hitVertices.OrderBy(v => v.z).ToList();

        (Vector3[] triVertices, int[] triangles) = GenerateTriangle(_hitVertices);
        UpdateMesh(triVertices, triangles);
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

    (Vector3[] triVertices, int[] triangles) GenerateTriangle(List<Vector3> vertices)
    {
        mesh = new Mesh();
        // create a new gameObject
        GameObject roof = new GameObject("roof");

        roof.AddComponent<MeshRenderer>();
        roof.AddComponent<MeshFilter>().mesh = mesh;

        Vector3[] triangleVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            triangleVertices[i] = vertices[i];
        }

        int[] triangle = new int[]
        {
            0,2,1,
            2,3,1
        };

        return (triangleVertices, triangle);

    }

    void UpdateMesh(Vector3[] vertices, int[] triangles)
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
