using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public List<GameObject> GetSpawnList(GameObject agent, List<GameObject> agentList, string checkPtName, int nearNum)
    {
        List<GameObject> nearestAgents = GetNearAgents(agent, agentList, nearNum);
        List<List<GameObject>> spawnPtList = new List<List<GameObject>>();
        foreach (GameObject nearAgent in nearestAgents)
        {
            spawnPtList.Add(GetChildren(nearAgent, filterName: checkPtName));
        }

        List<GameObject> spawnPts = FlattenList(spawnPtList);
        return spawnPts;
    }

    public List<GameObject> GetSpawnedAgents(GameObject spawnLayer)
    {
        return GetChildren(spawnLayer);
    }

    public bool CheckCollision(GameObject checkPoint, float checkDistance, LayerMask layerMask)
    {
        return Physics.CheckSphere(checkPoint.transform.position, checkDistance, layerMask);
    }

    public (bool isCollided, int count) CheckCollisionCount(List<GameObject> checkList, float checkDistance, LayerMask layerMask)
    {
        int checkCount = 0;
        foreach (GameObject checkPt in checkList)
        {
            if (CheckCollision(checkPt, checkDistance, layerMask))
            {
                checkCount++;
            }
        }

        bool isCollided = checkCount != 0;
        return (isCollided, checkCount);
    }

    public float EvalAgentBalance(List<GameObject> agents)
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



    public Vector3 GetRelativePos(GameObject child, GameObject host)
    {
        return host.transform.InverseTransformPoint(child.transform.position);
    }

    public GameObject GetParent(GameObject child)
    {
        return child.transform.parent.gameObject;
    }

    public static List<GameObject> GetChildren(GameObject parent, GameObject filter = null, string filterName = null)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child == filter) continue;
            else if (child.name == filterName) continue;
            children.Add(child);
        }
        return children;
    }

    public List<GameObject> GetGrandChildren(GameObject grandParent)
    {
        List<GameObject> parents = GetChildren(grandParent);
        List<List<GameObject>> childrenList = new List<List<GameObject>>();
        foreach (GameObject parent in parents)
        {
            childrenList.Add(GetChildren(parent));
        }
        List<GameObject> grandChildren = FlattenList(childrenList);
        return grandChildren;
    }


    public GameObject SearchChild(GameObject parent, string filterName)
    {
        GameObject[] foundChild = new GameObject[1];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child.name == filterName) foundChild[0] = child;
        }
        return foundChild[0];
    }

    public List<GameObject> GetNearAgents(GameObject agent, List<GameObject> agentPool, int nearNum)
    {
        Vector3 pos1 = agent.transform.localPosition;
        Dictionary<GameObject, float> distancesDict = new Dictionary<GameObject, float>();
        foreach (GameObject poolAgent in agentPool)
        {
            Vector3 pos2 = poolAgent.transform.localPosition;
            float distance = (pos1 - pos2).magnitude;
            distancesDict.Add(poolAgent, distance);
        }
        var smallestPairs = distancesDict.OrderBy(x => x.Value).Take(nearNum).ToList();
        List<GameObject> nearAgents = smallestPairs.Select(pairs => pairs.Key).ToList();
        return nearAgents;
    }

    public List<T> FlattenList<T>(List<List<T>> sourceList)
    {
        List<T> result = new List<T>();
        foreach (List<T> list in sourceList)
        {
            foreach (T item in list)
            {
                result.Add(item);
            }
        }
        return result;
    }

    public List<Vector3> GetPositions(List<GameObject> agents)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject agent in agents)
        {
            Vector3 position = agent.transform.localPosition;
            positions.Add(position);
        }
        return positions;
    }
}
