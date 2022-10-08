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

    private string projectionPlane = "x";
    private float OverhangeDistance = 3f;


    // Start is called before the first frame update
    void Start()
    {
        _projectedVertices = ProMeshUtilities.GetProjectedVertices(_resultMesh, projectionPlane, OverhangeDistance, "Spawn", "checkPoint");
        ProMeshUtilities.GenerateColliderAtVertices(_projectedVertices, _roofPointLayer); 
        GameObject roofParent = ProRoof.CreateContainerObj().roof;

        StartCoroutine(WaitBeforeRayCast(0.05f));

        IEnumerator WaitBeforeRayCast(float waitSecond)
        {
            yield return new WaitForSeconds(waitSecond);

            _hitVertices = ProMeshUtilities.GetRaycastCeilingVert(_projectedVertices, _ceiling);

            GameObject roof = ProRoof.CreateRoof(_hitVertices, _resultMesh, projectionPlane, OverhangeDistance, roofParent);
        }
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
