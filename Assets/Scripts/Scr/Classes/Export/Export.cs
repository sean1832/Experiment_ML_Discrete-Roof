using System.IO;
using UnityEditor;
using UnityEngine;

public class Export : MonoBehaviour
{
    public static (string prefabPath, string meshPath) ExportAsPrefab(GameObject rootObj, string path, string customName)
    {
        if (customName == string.Empty) customName = rootObj.name;

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

        return (prefabPath, roofMeshPath);
    }

    public static void ExportMeta(string data, string fileName, string directory)
    {
        string dataDir = $"{directory}/metadata";
        Directory.CreateDirectory(dataDir);
        string dataPath = $"{dataDir}/{fileName}_meta.json";
        dataPath = AssetDatabase.GenerateUniqueAssetPath(dataPath);
        File.WriteAllText(dataPath, data);

    }
}
