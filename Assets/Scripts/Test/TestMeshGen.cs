using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEditor;


public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;

    [SerializeField] private GameObject _spawnTest;
    // Start is called before the first frame update
    void Start()
    {
        string projectionPlane = "x";
        float OverhangeDistance = 3f;

        (GameObject roofPointLayer,GameObject targetRoofObj) = ProRoof.CreateContainerObj();
        GameObject ceiling = ProRoof.CreateCeiling(_resultMesh);

        List<Vector3> projectedVertices = ProMeshUtilities.GetProjectedVertices(_resultMesh, projectionPlane, OverhangeDistance, "checkPoint");
        ProMeshUtilities.GenerateColliderAtVertices(projectedVertices, roofPointLayer);

        StartCoroutine(WaitBeforeRayCast(0.05f));

        IEnumerator WaitBeforeRayCast(float waitSecond)
        {
            yield return new WaitForSeconds(waitSecond);
            List<Vector3> hitVertices = ProMeshUtilities.GetRaycastCeilingVert(projectedVertices, ceiling);
            GameObject roof = ProRoof.CreateRoof(hitVertices, _resultMesh, projectionPlane, OverhangeDistance, targetRoofObj);
            // insert roof into spawnLayer
            Utilities.SetParent(roof, _spawnTest);
            // insert structure into spawnLayer
            Utilities.SetParent(gameObject, _spawnTest);

            ProRoof.DestroyContainerObj(ceiling, roofPointLayer);
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
