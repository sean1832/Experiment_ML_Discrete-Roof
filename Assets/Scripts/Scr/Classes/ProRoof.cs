using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProRoof : MonoBehaviour
{
    #region Properties

    public GameObject ResultMesh { get; set; }
    public string SpawnLayerName { get; set; }
    public string CheckPtName { get; set; }

    #endregion

    #region private fields

    private GameObject _ceiling;
    private GameObject _roofPointLayer;
    

    #endregion


    public void GenerateRoof()
    {
        GenerateGameObjects();

    }

    private void GenerateGameObjects()
    {
        // generate ceiling


        // generate roofPoints
    }


    
}
