using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TestTrain : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] private GameObject _testPt;

    [Header("Agent Settings")] 
    [SerializeField] [Range(2,10)] private int _nearAgentCount = 5;
    [Header("General Parameters")]
    [SerializeField] private GameObject _sampleCheckPoint;
    [SerializeField] private LayerMask _agentLayerMask;
    


    private List<GameObject> _agents = new List<GameObject>();
    private GameObject _currentAgent;
    private List<List<GameObject>> _joints = new List<List<GameObject>>();
    private List<GameObject> _checkPoints = new List<GameObject>();
    private GameObject _checkPoint;

    // values
    private int _idx;
    private bool _isCollided;
    private string _checkPointName;

    void Start()
    {
        Init();
    }

    
    void Update()
    {
        EnableAgentCollider(_agents[0], false);
        GameObject cp = SearchChild(_agents[0], _checkPointName);
        _isCollided = CheckCollision(cp, 0.2f, _agentLayerMask);
        print($"isCollided: {_isCollided}");

    }
    private void InitCurrentAgent(int idx)
    {
        _currentAgent = _agents[idx];
    }

    // ========================================== Related Functions ==========================================
    private void Connect()
    {

    }

    private void SpawnLogic(bool isCollided, GameObject agent, GameObject spawnPoint, Vector3 rotation)
    {
        Spawn(agent, spawnPoint);
        if (!_isCollided)
        {
            Rotate(agent, rotation);
            _idx ++;
        }
    }

    private bool CheckCollision(GameObject checkPoint, float checkDistance, LayerMask layerMask)
    {
        return Physics.CheckSphere(GetGrandRelativePos(checkPoint), checkDistance, layerMask);
    }

    private void EnableAgentCollider(GameObject agent, bool state)
    {
        agent.GetComponent<MeshCollider>().enabled = state;
    }

    private void Rotate(GameObject agent, Vector3 rotation)
    {
        // add local rotation
        agent.transform.localEulerAngles += rotation;
    }

    private void Spawn(GameObject agent, GameObject spawnPoint)
    {
        // set position
        agent.transform.localPosition = spawnPoint.transform.localPosition;
        // set local rotation
        GameObject spawnParent = GetParent(spawnPoint);
        agent.transform.localEulerAngles = spawnParent.transform.localEulerAngles;
    }

    private List<GameObject> GetSpawnPoints(GameObject agent, List<GameObject> agentList)
    {
        List<GameObject> nearestAgents = GetNearAgents(agent, agentList, _nearAgentCount);
        List<List<GameObject>> spawnPointLists = new List<List<GameObject>>();
        foreach (GameObject nearAgent in nearestAgents)
        {
            spawnPointLists.Add(GetChildren(nearAgent, filterName:_checkPointName));
        }
        List<GameObject> spawnPoints = FlattenList(spawnPointLists);
        return spawnPoints;
    }

    private Vector3 GetRotation(int caseNum)
    {
        switch (caseNum)
        {
            case 0:
                return new Vector3(90,0,0); // left
            case 1:
                return new Vector3(-90, 0, 0); // right
            case 2:
                return new Vector3(0, 0, 90); // front
            case 3:
                return new Vector3(0, 0, -90); // back
            default:
                return Vector3.zero; // front -- no change
        }
    }

    // ========================================== General Functions ==========================================
    private Vector3 GetGrandRelativePos(GameObject grandChild)
    {
        return grandChild.transform.parent.parent.InverseTransformPoint(grandChild.transform.position);
    }

    private List<GameObject> ExcludeSelf(GameObject self, List<GameObject> others)
    {
        List<GameObject> excluded = new List<GameObject>();
        foreach (var item in others)
        {
            if (item.name == self.name) continue;
            excluded.Add(item);
        }
        return excluded;
    }

    private List<T> FlattenList<T>(List<List<T>> sourceList)
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

    private List<GameObject> GetNearAgents(GameObject agent, List<GameObject> agentsPool, int nearNum)
    {
        Vector3 pos1 = agent.transform.localPosition;
        Dictionary<GameObject, float> distancesDict = new Dictionary<GameObject, float>();
        foreach (GameObject poolAgent in agentsPool)
        {
            Vector3 pos2 = poolAgent.transform.localPosition;
            float distance = (pos1 - pos2).magnitude;
            distancesDict.Add(poolAgent, distance);
        }
        var smallestPairs = distancesDict.OrderBy(x => x.Value).Take(nearNum).ToList();
        List<GameObject> nearAgents = smallestPairs.Select(pairs => pairs.Key).ToList();
        return nearAgents;
    }


    private GameObject GetParent(GameObject child)
    {
        return child.transform.parent.gameObject;
    }

    private GameObject SearchChild(GameObject parent, string filterName)
    {
        GameObject[] foundChild = new GameObject[1];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child.name == filterName) foundChild[0] = child;
        }
        return foundChild[0];
    }

    private List<GameObject> GetChildren(GameObject parent, GameObject filter = null, string filterName = null)
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

    // ========================================== Init Functions ==========================================
    private void Init()
    {
        InitFields();
    }

    private void InitFields()
    {
        _agents = GetChildren(gameObject);
        _checkPointName = _sampleCheckPoint.name;
        foreach (GameObject agent in _agents)
        {
            _joints.Add(GetChildren(agent, filterName:_checkPointName));
            _checkPoints.Add(SearchChild(agent,_checkPointName));
        }
        _idx = 0;
    }

    
}
