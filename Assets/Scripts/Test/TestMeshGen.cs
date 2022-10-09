using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEditor;


public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;

    // Start is called before the first frame update
    void Start()
    {
        string projectionPlane = "x";
        float OverhangeDistance = 3f;

        (GameObject roofPointLayer,GameObject targetRoofObj, GameObject exportPackage) = ProRoof.CreateContainerObj();
        GameObject ceiling = ProRoof.CreateCeiling(_resultMesh);

        List<Vector3> projectedVertices = ProMeshUtilities.GetProjectedVertices(_resultMesh, projectionPlane, OverhangeDistance, "checkPoint");
        ProMeshUtilities.GenerateColliderAtVertices(projectedVertices, roofPointLayer);

        StartCoroutine(WaitBeforeRayCast(0.05f));

        IEnumerator WaitBeforeRayCast(float waitSecond)
        {
            yield return new WaitForSeconds(waitSecond);
            List<Vector3> hitVertices = ProMeshUtilities.GetRaycastCeilingVert(projectedVertices, ceiling);
            ProRoof.DestroyContainerObj(ceiling, roofPointLayer);

            GameObject roof = ProRoof.ConstructRoof(hitVertices, _resultMesh, projectionPlane, OverhangeDistance, targetRoofObj);
            Utilities.SetParent(roof, exportPackage);
            Utilities.SetParent(_resultMesh, exportPackage);
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
