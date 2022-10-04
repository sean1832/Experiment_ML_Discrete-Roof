using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;
    [SerializeField] private GameObject _ceiling;
    [SerializeField] private GameObject _roofPointLayer;

    private List<Vector3> _hitVertices;

    private bool _isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _isStarted = true;
        StartCoroutine(WaitBeforeRayCase());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaitBeforeRayCase()
    {
        List<Vector3> vertices = Utilities.GetAllRoofVertices(_resultMesh, "Spawn", "checkPoint");
        Actions.GenerateGameObject(vertices, _roofPointLayer);
        yield return new WaitForSeconds(0.05f);
        _hitVertices = Utilities.GetTopVertices(vertices, _ceiling);

        List<Vector3> movedVertices = Utilities.MoveVertices(_hitVertices, new Vector3(1, 0, 0), 20f);
        _hitVertices.AddRange(movedVertices);
    }


    private void OnDrawGizmos()
    {
        if (!_isStarted) return;
        if (_hitVertices == null) return;
        foreach (var vertex in _hitVertices)
        {
            Gizmos.DrawSphere(vertex, 0.2f);
        }
    }
}
