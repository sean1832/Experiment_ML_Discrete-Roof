using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    public GameObject ChoseSpawnPt(int caseNum ,List<GameObject> spawnList)
    {
        return spawnList[caseNum];
    }

    public Vector3 ChoseRotation(int caseNum)
    {
        switch (caseNum)
        {
            case 0:
                return new Vector3(90, 0, 0); // left
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
}
