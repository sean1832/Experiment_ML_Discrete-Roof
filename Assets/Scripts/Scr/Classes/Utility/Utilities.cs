using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    #region GetSpawnList
    public List<GameObject> GetSpawnList(GameObject agent, List<GameObject> agentList, string checkPtName, int nearNum)
    {
        List<GameObject> nearestAgents = GetNearAgents(agent, agentList, nearNum);

        List<GameObject> spawnPts = GetJoints(nearestAgents, checkPtName);

        return spawnPts;
        //List<List<GameObject>> spawnPtList = new List<List<GameObject>>();
        //foreach (GameObject nearAgent in nearestAgents)
        //{
        //    spawnPtList.Add(GetChildren(nearAgent, filterName: checkPtName));
        //}

        //List<GameObject> spawnPts = Utilities.FlattenList(spawnPtList);

    }
    #endregion

    #region GetJoints
    public static List<GameObject> GetJoints(List<GameObject> agents, string checkPointName)
    {
        List<List<GameObject>> jointList = new List<List<GameObject>>();

        foreach (GameObject child in agents)
        {
            List<GameObject> joints = Utilities.GetChildren(child, filterName: checkPointName);
            jointList.Add(joints);
        }

        return Utilities.FlattenList(jointList);
    }
    #endregion

    #region GetSpawnedAgents
    public List<GameObject> GetSpawnedAgents(GameObject spawnLayer)
    {
        return GetChildren(spawnLayer);
    }
    #endregion

    #region CheckCollision & CheckCollisionCount
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
    #endregion


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
        List<GameObject> grandChildren = Utilities.FlattenList(childrenList);
        return grandChildren;
    }


    public static GameObject SearchChild(GameObject parent, string filterName)
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

    public static List<T> FlattenList<T>(List<List<T>> sourceList)
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

    public static List<Vector3> CullDuplicate(List<Vector3> vertices, float tolerance = 0.001f, int maxIterationCount = 10000)
    {
        // absolute culling
        List<Vector3> distinctList = vertices.Distinct().ToList();

        // tolerance culling
        int count = 0;
        for (int i = 0; i < distinctList.Count; i++)
        {
            for (int j = i + 1; j < distinctList.Count; j++)
            {
                count++;
                if (count > maxIterationCount) Debug.LogError("vertices too much!");

                float distance = Utilities.GetDistance(distinctList[i], distinctList[j]);
                if (distance<tolerance)
                {
                    distinctList.RemoveAt(j);
                }
            }
        }

        return distinctList;
    }

    public static (float x, float y, float z, Vector3 center) GetBounds(List<GameObject> objects, string maxOrMin)
    {
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        List<float> z = new List<float>();

        foreach (GameObject obj in objects)
        {
            x.Add(obj.transform.position.x);
            y.Add(obj.transform.position.y);
            z.Add(obj.transform.position.z);
        }

        Vector3 center = new Vector3(x.Average(), y.Average(), z.Average());

        switch (maxOrMin)
        {
            case "max" or "Max":
                return (x.Max(), y.Max(), z.Max(), center);
            case "min" or "Min":
                return (x.Min(), y.Min(), z.Min(), center);
            default:
                throw new ArgumentException($"MinOrMax value: ({maxOrMin}) not accepted, please enter either min or max");
        }
    }
    public static (float x, float y, float z) GetBounds(List<Vector3> vertices, string maxOrMin)
    {
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        List<float> z = new List<float>();

        foreach (Vector3 vertex in vertices)
        {
            x.Add(vertex.x);
            y.Add(vertex.y);
            z.Add(vertex.z);
        }

        switch (maxOrMin)
        {
            case "max" or "Max":
                return (x.Max(), y.Max(), z.Max());
            case "min" or "Min":
                return (x.Min(), y.Min(), z.Min());
            default:
                throw new ArgumentException($"MinOrMax value: ({maxOrMin}) not accepted, please enter either min or max");
        }
    }

    public static void SetParent(List<GameObject> children, GameObject parent)
    {
        foreach (var child in children)
        {
            child.transform.SetParent(parent.transform);
        }
    }

    public static void SetParent(GameObject child, GameObject parent)
    {
        child.transform.SetParent(parent.transform);
    }

    public static float GetDistance(Vector3 ptA, Vector3 ptB)
    {
        return (ptA - ptB).magnitude;
    }
}
