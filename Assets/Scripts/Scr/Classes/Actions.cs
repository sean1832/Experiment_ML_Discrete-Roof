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
        string fullPath = path + customName + ".prefab";
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(copiedObj, fullPath, InteractionMode.UserAction);
        Destroy(copiedObj);
    }

    public void GenerateRoof(List<GameObject> agents)
    {
        
    }

    public static void GenerateGameObject(List<Vector3> vertices, GameObject parent)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject newObj = new GameObject($"RoofPoint_{i}");
            newObj.AddComponent<SphereCollider>().radius = 0.2f;
            newObj.transform.position = vertices[i];
            newObj.transform.parent = parent.transform;
        }
    }
}
