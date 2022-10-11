using UnityEngine;

public class ExportUtilities : MonoBehaviour
{
    public static string GetJsonData(string prefabName, float reward, float roofArea, int collidedCount, int redundantActions, float height)
    {
        ResultObject result = new ResultObject()
        {
            Name = prefabName,
            CumulativeReward = reward,
            RoofArea = roofArea,
            TargetReachCount = collidedCount,
            RedundantAction = redundantActions,
            Height = height
        };
        string json = JsonUtility.ToJson(result,true);
        return json;
    }
}
