using UnityEngine;

public class ExportUtilities : MonoBehaviour
{
    public static string GetJsonData(string prefabName, float reward, float roofArea, int collidedCount, float height)
    {
        ResultObject result = new ResultObject()
        {
            Name = prefabName,
            CumulativeReward = reward,
            RoofArea = roofArea,
            TargetReachCount = collidedCount,
            Height = height
        };
        string json = JsonUtility.ToJson(result,true);
        return json;
    }
}
