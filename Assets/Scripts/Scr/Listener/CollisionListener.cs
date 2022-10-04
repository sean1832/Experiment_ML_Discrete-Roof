using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        GameObject root = GameObject.Find("Assets");
        if (root.GetComponent<Train>() != null)
        {
            root.GetComponent<Train>().CollisionDetectionEnter(this);
        }
    }
}
