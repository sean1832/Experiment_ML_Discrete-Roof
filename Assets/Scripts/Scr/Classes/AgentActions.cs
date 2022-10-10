using System.IO;
using UnityEditor;
using UnityEngine;

public class AgentActions : MonoBehaviour
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
}
