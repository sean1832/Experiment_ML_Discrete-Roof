using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;
    // Start is called before the first frame update
    void Start()
    {
        string projectionPlane = "x";
        float OverhangeDistance = 3f;

        (GameObject roofPointLayer,GameObject roofParent) = ProRoof.CreateContainerObj();
        GameObject ceiling = ProRoof.CreateCeiling(_resultMesh);


        List<Vector3> projectedVertices = ProMeshUtilities.GetProjectedVertices(_resultMesh, projectionPlane, OverhangeDistance, "Spawn", "checkPoint");
        ProMeshUtilities.GenerateColliderAtVertices(projectedVertices, roofPointLayer); 
        

        StartCoroutine(WaitBeforeRayCast(0.05f));

        IEnumerator WaitBeforeRayCast(float waitSecond)
        {
            yield return new WaitForSeconds(waitSecond);

            List<Vector3> hitVertices = ProMeshUtilities.GetRaycastCeilingVert(projectedVertices, ceiling);

            GameObject roof = ProRoof.CreateRoof(hitVertices, _resultMesh, projectionPlane, OverhangeDistance, roofParent);
        }
    }
    


    //private void OnDrawGizmos()
    //{
    //    if (!_isStarted) return;
    //    if (_hitVertices == null) return;
    //    foreach (var vertex in _hitVertices)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(vertex, 0.2f);
    //    }

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(_hitVertices[2], 0.2f);
    //}

}
