using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    public static float AgentBalance(List<GameObject> agents)
    {
        int agentsZPosCount = 0;
        int agentsZNegCount = 0;
        int agentsXPosCount = 0;
        int agentsXNegCount = 0;
        foreach (var agent in agents)
        {
            float agentZ = agent.transform.position.z;
            float agentX = agent.transform.position.x;

            if (agentZ >= 0) agentsZPosCount++;
            else agentsZNegCount++;

            if (agentX >= 0) agentsXPosCount++;
            else agentsXNegCount++;
        }
        int zDifference = math.abs(agentsZPosCount - agentsZNegCount);
        int xDifference = math.abs(agentsXPosCount - agentsXNegCount);

        float zScore = (float)zDifference / (float)agents.Count;
        float xScore = (float)xDifference / (float)agents.Count;

        float score = (zScore + xScore) / 2;

        return score;
    }

    public static (float score, float area) SurfaceArea(GameObject obj, float multiplier = 1.0f)
    {
        Mesh mesh = ProMeshUtilities.GetMesh(obj);
        float area = ProMeshAnalyse.CalcSurfaceArea(mesh);
        float score = area * multiplier;

        return (score, area);
    }
}
