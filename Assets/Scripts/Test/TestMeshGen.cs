using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEditor;


public class TestMeshGen : MonoBehaviour
{
    //[SerializeField] private GameObject _resultMesh;

    private ProRoof _proRoof;

    private bool _isStarted = false;

    private List<Vector3> _hitVertices;

    // Start is called before the first frame update
    void Start()
    {
        _isStarted = true;
        _proRoof = gameObject.AddComponent<ProRoof>();
        

        

        GameObject package = _proRoof.CreateRoof(gameObject);
        Destroy(package);
        //StartCoroutine(ExecuteAfter());

        //IEnumerator ExecuteAfter()
        //{
        //    yield return new WaitForSeconds(0.05f);
        //    GameObject roof = Utilities.SearchChild(package, "roof");
        //    Mesh roofMesh = ProMeshUtilities.GetMesh(roof);
        //    print(roofMesh.name);
            
        //}
        
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
    //    Gizmos.DrawSphere(_hitVertices[1], 0.2f);
    //    Gizmos.DrawSphere(_hitVertices[0], 0.2f);
    //}

}
