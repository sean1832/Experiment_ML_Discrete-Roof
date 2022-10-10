using System.IO;
using UnityEditor;
using UnityEngine;

public class Export : MonoBehaviour
{
    public static void ExportAsPrefab(GameObject rootObj, string path, string customName, bool isResults, int episode)
    {
        if (customName == string.Empty) customName = rootObj.name;
        if (isResults)
        {
            path = $"{path}/Results";
        }
        else
        {
            path = $"{path}/Progress";
            customName = $"{customName}_eps({episode})";
        }

        GameObject copiedObj = Instantiate(rootObj);
        Directory.CreateDirectory(path); // create path if not exist
        string prefabPath = $"{path}/{customName}.prefab";
        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

        // export roof mesh
        GameObject roofObj = Utilities.SearchChild(copiedObj, "roof");
        Mesh roofMesh = roofObj.GetComponent<MeshFilter>().mesh;
        string meshPath = $"{path}/mesh";
        Directory.CreateDirectory(meshPath); // create path if not exist
        string roofMeshPath = $"{meshPath}/{customName}.asset";
        roofMeshPath = AssetDatabase.GenerateUniqueAssetPath(roofMeshPath);
        // save mesh
        AssetDatabase.CreateAsset(roofMesh, roofMeshPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // save prefab
        PrefabUtility.SaveAsPrefabAssetAndConnect(copiedObj, prefabPath, InteractionMode.UserAction);
        Destroy(copiedObj);
    }

    public static void ExportMetadata()
    {

    }
}
