using System.Collections;
using System.Collections.Generic;
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
        agent.transform.position = _utilities.GetRelativePos(spawnPoint, gameObject);
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
        string prefabPath = path + customName + ".prefab";
        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

        // export roof mesh
        GameObject roofObj = Utilities.SearchChild(copiedObj, "roof");
        Mesh roofMesh = roofObj.GetComponent<MeshFilter>().mesh;
        string roofMeshPath = $"{path}/mesh/{customName}.asset";
        roofMeshPath = AssetDatabase.GenerateUniqueAssetPath(roofMeshPath);
        // save mesh
        AssetDatabase.CreateAsset(roofMesh, roofMeshPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // save prefab
        PrefabUtility.SaveAsPrefabAssetAndConnect(copiedObj, prefabPath, InteractionMode.UserAction);
        Destroy(copiedObj);
    }


}
