using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Actions : MonoBehaviour
{
    private Utilities _utilities;

    public void Init()
    {
        _utilities = gameObject.GetComponent<Utilities>();
    }
    public void Spawn(GameObject agent, GameObject spawnPoint)
    {
        // set position
        //agent.transform.localPosition = _utilities.GetGrandRelativePos(spawnPoint);
        agent.transform.position = _utilities.GetRelativePos(spawnPoint, gameObject);
        //print($"relative position: {_utilities.GetRelativePos(spawnPoint, gameObject)}");
        // set rotation
        GameObject spawnParent = _utilities.GetParent(spawnPoint);
        agent.transform.localEulerAngles = spawnParent.transform.localEulerAngles;
    }

    public void Rotate(GameObject agent, Vector3 rotation)
    {
        agent.transform.localEulerAngles += rotation;
    }

    public void EnableAgentCollider(GameObject agent, bool state)
    {
        agent.GetComponent<BoxCollider>().enabled = state;
    }

    public void ExportAsPrefab(GameObject rootObj, string path, string customName)
    {
        GameObject copiedObj = Instantiate(rootObj);
        if (customName == string.Empty) customName = rootObj.name;
        string fullPath = path + customName + ".prefab";
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(copiedObj, fullPath, InteractionMode.UserAction);
        Destroy(copiedObj);
    }

    public void GenerateRoof(List<GameObject> agents)
    {

    }

}
