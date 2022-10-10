using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEditor;


public class TestMeshGen : MonoBehaviour
{
    [SerializeField] private GameObject _resultMesh;

    private ProRoof _proRoof;

    // Start is called before the first frame update
    void Start()
    {
        //_proRoof = gameObject.AddComponent<ProRoof>();
        //var exportPackage = _proRoof.CreateRoof(_resultMesh, true);

        GameObject roof = Utilities.SearchChild(_resultMesh, "roof");

        Mesh mesh = ProMeshUtilities.GetMesh(roof);

        float area = ProMeshAnalyse.GetMeshSurfaceArea(mesh);

        print(area);
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
