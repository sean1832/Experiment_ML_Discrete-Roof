using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;
    [SerializeField] private GameObject _ceiling;


    private List<GameObject> _resultChildren = new List<GameObject>();
    private List<Vector3> vertices = new List<Vector3>();

    private List<Vector3> _hitVertices;

    private bool _isStarted = false;

    private RaycastHit _hitInfo;
    // Start is called before the first frame update
    void Start()
    {
        _isStarted = true;
        // get children
        _resultChildren = Utilities.GetChildren(_resultMesh);

        // add positions to vertices
        List<GameObject> joints = Utilities.GetJoints(_resultChildren, "checkPoint");
        foreach (GameObject joint in joints)
        {
            vertices.Add(joint.transform.position);
        }

        // project vertices on a plane
        vertices = Utilities.ProjectVertices(vertices, "x", -5f);
        vertices = Utilities.CullDuplicate(vertices);

        // generate GameObjects at vertices positions and apply sphere collider
        GameObject newJointsParent = new GameObject("JointsParent");
        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject newObj = new GameObject($"joint{i}");
            newObj.AddComponent<SphereCollider>().radius = 0.2f;
            newObj.transform.position = vertices[i];
            newObj.transform.parent = newJointsParent.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {

        _hitVertices = new List<Vector3>();
        // ray casting
        foreach (var vertex in vertices)
        {
            _hitInfo = new RaycastHit();
            Ray ray = new Ray(vertex, Vector3.up);
            if (!Physics.Raycast(ray, out _hitInfo, 20f)) continue;
            if (_hitInfo.collider.gameObject == _ceiling)
            {
                _hitVertices.Add(vertex);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isStarted) return;
        foreach (Vector3 vertex in _hitVertices)
        {
            Gizmos.DrawSphere(vertex, 0.2f);
        }
    }
}
