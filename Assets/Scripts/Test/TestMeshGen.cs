using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;
    [SerializeField] private LayerMask _layerMask;

    private List<GameObject> _resultChildren = new List<GameObject>();
    private List<Vector3> vertices = new List<Vector3>();

    private RaycastHit _hitInfo;
    // Start is called before the first frame update
    void Start()
    {
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
        _hitInfo = new RaycastHit();
        Ray ray = new Ray(vertices[15], Vector3.down);
        if (Physics.Raycast(ray, out _hitInfo, 20f))
        {
            print($"hit! hit info is: {_hitInfo.collider.gameObject.name}");
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 vertex in vertices)
        {
            Gizmos.DrawSphere(vertex, 0.2f);
            Gizmos.DrawRay(vertices[15], Vector3.down * _hitInfo.distance);
        }
    }
}
