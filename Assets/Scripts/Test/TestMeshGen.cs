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

    private List<Vector3> _projectedVertices;
    private List<Vector3> _hitVertices;
    private bool _isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _projectedVertices = ProMeshUtilities.GetProjectedVertices(_resultMesh, "x", 3f,"Spawn", "checkPoint");
        ProMeshUtilities.GenerateObjAtVertices(_projectedVertices, _roofPointLayer);
        _isStarted = true;
        StartCoroutine(WaitBeforeRayCast(0.05f));
    }

    IEnumerator WaitBeforeRayCast(float waitSecond)
    {
        yield return new WaitForSeconds(waitSecond);
        _hitVertices = ProMeshUtilities.GetRaycastCeilingVert(_projectedVertices, _ceiling);

        List<Vector3> movedVertices = ProMeshUtilities.GetOffsetVertices(_hitVertices, new Vector3(1, 0, 0), 20f);
        _hitVertices.AddRange(movedVertices);
        _hitVertices = _hitVertices.OrderBy(v => v.z).ToList();


        GameObject roofParent = new GameObject("roof");
        ProMeshConstruct.ConstructMesh(_hitVertices, false, roofParent);
        ProMeshConstruct.ConstructMesh(_hitVertices,true, roofParent);
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

}
