using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

public class Train : Agent
{

    #region Inputs Parameters

    [Header("Agent parameter")]
    [SerializeField][Range(0, 0.3f)] private float _checkPtRadius = 0.2f;
    [SerializeField][Range(1f, 10f)] private float _radiusMultiplier = 2;
    [SerializeField] private bool _enableWallCollision = false;
    [SerializeField][Range(3,15)] private int _nearNum = 4;
    [SerializeField] private bool _evaluateAgentBalance = false;
    [SerializeField] private bool _enableRoofGeneration = false;


    [Header("Observation parameters")]
    [SerializeField] private GameObject _roofJointsRoot;

    [Header("General parameters")]
    [SerializeField] private LayerMask _agentLayerMask;
    [SerializeField] private GameObject _checkPointSample;
    [SerializeField] private GameObject _spawnLayer;
    [SerializeField] private Renderer _ground;

    [Header("Sample export parameters")]
    [SerializeField] private bool _enableSampleExport = false;
    [SerializeField] private int _exportEpisodeStep = 10000;

    [Header("Export Parameters")]
    [SerializeField] private bool _enableExport = false;
    [SerializeField] private bool  _isFinalResults = false;
    [SerializeField] private string _exportDirectory = "Assets/Prefabs/Outputs";
    [SerializeField] private string _exportPrefabName = "result";

    #endregion

    #region Fields

    // agents
    private string _checkPtnName;
    private List<GameObject> _agents;
    private GameObject _currentAgent;
    private List<GameObject> _spawnedAgents;
    private List<GameObject> _spawnList = new List<GameObject>();
    private List<Vector3> _agentInitPos = new List<Vector3>();

    // observation
    private List<GameObject> _roofJoints = new List<GameObject>();
    private bool _isWallCollided = false;

    // ml Param
    private int _idx;
    private int _targetEpisodeCount;

    // classes
    private AgentActions _agentActions;
    private Utilities _utilities;
    private Decision _decision;
    private ProRoof _proRoof;


    private bool _isStarted = false;

    #endregion

    #region Init Functions
    private void Init()
    {
        InitClass();
        InitFields();
        _agents[0].transform.parent = _spawnLayer.transform;
    }

    private void InitClass()
    {
        // add and assign class
        _agentActions = gameObject.AddComponent<AgentActions>();
        _utilities = gameObject.AddComponent<Utilities>();
        _decision = gameObject.AddComponent<Decision>();
        _proRoof = gameObject.AddComponent<ProRoof>();
        
        // init class
        _agentActions.Init();
    }

    private void InitFields()
    {
        _isStarted = true;

        _checkPtnName = _checkPointSample.name;

        _agents = Utilities.GetChildren(gameObject, _spawnLayer);
        _agentInitPos = _utilities.GetPositions(_agents);

        _roofJoints = _utilities.GetGrandChildren(_roofJointsRoot);

        CreateDummy();
        _idx = 1;
        _targetEpisodeCount = _exportEpisodeStep;
    }
    #endregion

    #region MonoBehavior Functions
    private void Start()
    {
        Init();
    }
    #endregion

    #region Training Logic

    public override void OnEpisodeBegin()
    {
        if (_idx < _agents.Count && _idx != 1 && !_isWallCollided)
        {
            ResetCurrentAgent();
        }
        else
        {
            ResetAllAgents();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // observe agent center point position
        Vector3 agentPos = _currentAgent.transform.localPosition;
        sensor.AddObservation(agentPos);

        // observe each potential spawn points positions
        foreach (GameObject spawnPt in _spawnList)
        {
            sensor.AddObservation(spawnPt.transform.position);
        }

        // observe agent each joint positions
        List<GameObject> agentJoints = Utilities.GetChildren(_currentAgent, filterName: _checkPtnName);
        foreach (GameObject agentJoint in agentJoints)
        {
            //Vector3 agentJointPos = _utilities.GetRelativePos(agentJoint, gameObject);
            sensor.AddObservation(agentJoint.transform.position);
        }

        // observe each roof connection positions
        foreach (GameObject roofJoint in _roofJoints)
        {
            //Vector3 roofJointPos = _utilities.GetRelativePos(roofJoint, gameObject);
            sensor.AddObservation(roofJoint.transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // reset wall collided
        _isWallCollided = false;

        int spawnChoice = actions.DiscreteActions[0];
        int rotationChoice = actions.DiscreteActions[1];

        #region Actions

        // turn off current agent collision (prevent self collision detection)
        _agentActions.EnableAgentCollider(_currentAgent, false);

        GameObject checkPt = Utilities.SearchChild(_currentAgent, _checkPtnName);

        GameObject spawnPt = _decision.ChoseSpawnPt(spawnChoice, _spawnList);
        _agentActions.Spawn(_currentAgent, spawnPt);

        Vector3 rotation = _decision.ChoseRotation(rotationChoice);
        _agentActions.Rotate(_currentAgent, rotation);

        #endregion

        #region Evaluation

        // check agent collision
        bool isAgentCollided = _utilities.CheckCollision(checkPt, _checkPtRadius, _agentLayerMask);

        // check roof joint collision
        (bool isRoofCollided, int collidedCount) = _utilities.CheckCollisionCount(_roofJoints, _checkPtRadius * _radiusMultiplier, _agentLayerMask);

        if (isAgentCollided) // failed to connect = Agent is collided
        {
            AddReward(-2);
            EndEpisode();
        }
        else if (isRoofCollided) // successfully connect and reach goal
        {
            #region Finish Actions
            if (_idx == _agents.Count - 1) // connected all and finish
            {
                print("success");
                AddReward(+50);
                AddReward(collidedCount * 2);
                SetMaterial(_ground, Color.green);

                if (_evaluateAgentBalance)
                {
                    float multiplier = -20f;
                    float score = Evaluation.AgentBalance(_agents) * multiplier;

                    AddReward(score);
                }

                if (_enableRoofGeneration)
                {
                    GameObject exportPackage = _proRoof.CreateRoof(_spawnLayer);
                    StartCoroutine(ExecuteAfter());
                    IEnumerator ExecuteAfter()
                    {
                        yield return new WaitForSeconds(0.05f);
                        // other script execute after
                        GameObject roof = Utilities.SearchChild(exportPackage, "roof");
                        float score = Evaluation.SurfaceArea(roof, 0.02f);

                        AddReward(score);

                        // export
                        if (_enableExport)
                        {
                            Export.ExportAsPrefab(exportPackage, _exportDirectory, _exportPrefabName, _isFinalResults, CompletedEpisodes);
                        }

                        // delete
                        Destroy(exportPackage);
                    }
                }

                if (_enableExport && !_enableRoofGeneration) // export geometry as prefab
                {
                    Export.ExportAsPrefab(_spawnLayer, _exportDirectory, _exportPrefabName, _isFinalResults, CompletedEpisodes);
                }
            }
            
            Continue();

            #endregion
        }
        else if (_idx == _agents.Count - 1) // successfully connect but never reached goal
        {
            AddReward(-20);
            Continue();
        }
        else // successfully connect but not reach goal yet
        {
            AddReward(+1);
            Continue();
        }
        #endregion

    }

    #endregion

    #region Heuristic
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discActions = actionsOut.DiscreteActions;

        int spawnCase = 4;

        if (Input.GetKey(KeyCode.Keypad1)) spawnCase = 0;
        else if (Input.GetKey(KeyCode.Keypad2)) spawnCase = 1;
        else if (Input.GetKey(KeyCode.Keypad3)) spawnCase = 2;
        else if (Input.GetKey(KeyCode.Keypad4)) spawnCase = 3;

        discActions[1] = spawnCase;
    }


    #endregion

    #region Listener
    public void CollisionDetectionEnter(CollisionListener listener)
    {
        if (!_enableWallCollision) return;
        _isWallCollided = true;
        AddReward(-20);
        SetMaterial(_ground, Color.red);
        EndEpisode();
    }
    #endregion

    #region Local Functions

    private void ExportSamples(GameObject sample)
    {
        if (!_enableSampleExport) return;
        if (CompletedEpisodes < _targetEpisodeCount) return;
        print("exported");
        //Export.ExportAsPrefab(sample, _exportDirectory, _exportPrefabName, _isFinalResults, CompletedEpisodes);
        _targetEpisodeCount += _exportEpisodeStep;
    }

    private void SetMaterial(Renderer renderer, Color color)
    {
        renderer.material.color = color;
    }

    private void Continue()
    {
        // set agent parent to spawn layer
        _currentAgent.transform.parent = _spawnLayer.transform;
        // turn collision back on
        _agentActions.EnableAgentCollider(_currentAgent, true);
        _idx++;
        EndEpisode();
    }

    private void ResetCurrentAgent()
    {
        _spawnedAgents = _utilities.GetSpawnedAgents(_spawnLayer);
        _currentAgent = _agents[_idx];
        GameObject previousAgent = _agents[_idx - 1];

        // replace dummy spawn points
        List<GameObject> foundList = _utilities.GetSpawnList(previousAgent, _spawnedAgents, _checkPtnName, _nearNum);
        for (int i = 0; i < foundList.Count; i++)
        {
            _spawnList[i] = foundList[i];
        }
    }

    private void ResetAllAgents()
    {
        for (int i = 0; i < _agents.Count; i++)
        {
            GameObject agent = _agents[i];
            // reset agent locations
            agent.transform.localPosition = _agentInitPos[i];
            // reset parents hierarchy 
            agent.transform.SetParent(gameObject.transform);
        }

        // sort agent by their names
        _agents.Sort((GameObject x, GameObject y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
        // insert dummy pts
        CreateDummy();
        // set index back to 1
        _idx = 1;
        _currentAgent = _agents[_idx];
    }

    private void CreateDummy()
    {
        _spawnList.Clear();
        int spawnNum = _nearNum * 2;
        for (int i = 0; i < spawnNum; i++)
        {
            _spawnList.Add(Utilities.SearchChild(_agents[0], "pt2"));
        }
    }

    #endregion

    #region Debug Functions

    private void OnDrawGizmos()
    {
        if (!_isStarted) return;
        GameObject agentCheckPt = Utilities.SearchChild(_currentAgent, _checkPtnName);
        DrawSphere(Color.red, agentCheckPt.transform.position, _checkPtRadius);

        foreach (GameObject joint in _roofJoints)
        {
            DrawSphere(Color.blue, joint.transform.position, _checkPtRadius * _radiusMultiplier);
        }
    }
    private void DrawSphere(Color color, Vector3 center, float radius)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(center, radius);
    }

    #endregion

}
