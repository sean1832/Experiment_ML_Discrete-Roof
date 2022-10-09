using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ProRoof : MonoBehaviour
{
    public GameObject CreateRoof(GameObject resultedStructure)
    {
        string projectionPlane = "x";
        float overhangDistance = 3f;

        (GameObject roofPointLayer, GameObject targetRoofObj, GameObject exportPackage) = ProRoof.CreateContainerObj();
        GameObject ceiling = ProRoof.CreateCeiling(resultedStructure);

        List<Vector3> projectedVertices = ProMeshUtilities.GetProjectedVertices(resultedStructure, projectionPlane, overhangDistance, "checkPoint");
        ProMeshUtilities.GenerateColliderAtVertices(projectedVertices, roofPointLayer);

        StartCoroutine(WaitBeforeRayCast(0.05f));

        IEnumerator WaitBeforeRayCast(float waitSecond)
        {
            yield return new WaitForSeconds(waitSecond);
            List<Vector3> hitVertices = ProMeshUtilities.GetRaycastCeilingVert(projectedVertices, ceiling);
            ProRoof.DestroyContainerObj(ceiling, roofPointLayer);

            GameObject roof = ProRoof.ConstructRoof(hitVertices, resultedStructure, projectionPlane, overhangDistance, targetRoofObj);
            Utilities.SetParent(roof, exportPackage);
            Utilities.SetParent(resultedStructure, exportPackage);
        }

        return exportPackage;
    }






    private static GameObject ConstructRoof(List<Vector3> hitVertices, GameObject resultMesh, string projectionPlane, float overHangDistance, GameObject roofParent)
    {
        Vector3 direction;
        float boundsValMax = 0;
        float boundsValMin = 0;
        List<GameObject> joints = GetJoints(resultMesh);


        switch (projectionPlane)
        {
            case "x":
                direction = new Vector3(1, 0, 0);
                boundsValMax = Utilities.GetBounds(joints, "max").x;
                boundsValMin = Utilities.GetBounds(joints, "min").x;
                break;
            case "y":
                direction = new Vector3(0, 1, 0);
                boundsValMax = Utilities.GetBounds(joints, "max").y;
                boundsValMin = Utilities.GetBounds(joints, "min").y;
                break;
            case "z":
                direction = new Vector3(0, 0, 1);
                boundsValMax = Utilities.GetBounds(joints, "max").z;
                boundsValMin = Utilities.GetBounds(joints, "min").z;
                break;
            default:
                throw new ArgumentException(
                    $"projection plane value: ({projectionPlane}) is invalid, please enter a valid value. Valid value are: x, y, z");
        }


        float offsetDistance = math.abs(boundsValMax - boundsValMin) + overHangDistance+2; // +2 here

        List<Vector3> movedVertices = ProMeshUtilities.GetOffsetVertices(hitVertices, direction, offsetDistance);
        hitVertices.AddRange(movedVertices);
        hitVertices = hitVertices.OrderBy(v => v.z).ToList();

        GameObject roofUp = ProMeshConstruct.ConstructMesh(hitVertices, false, roofParent);
        GameObject roofDown = ProMeshConstruct.ConstructMesh(hitVertices, true, roofParent);

        List<GameObject> roofList = new List<GameObject>() { roofUp, roofDown };

        GameObject roof = ProMeshUtilities.CombineMeshes(roofList, roofParent);

        Destroy(roofUp);
        Destroy(roofDown);
        return roof;
    }

    private static GameObject CreateCeiling(GameObject resultMesh)
    {
        List<GameObject> joints = GetJoints(resultMesh);

        // create ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ceiling.name = "ceiling";

        (float xMax, float yMax, float zMax, Vector3 center) = Utilities.GetBounds(joints, "max");

        // adjust height, location and size of ceiling
        ceiling.transform.position = center; // center
        ceiling.transform.position += new Vector3(0, yMax+10, 0); // height

        float scaleVal = 3f;
        ceiling.transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);

        // rotate 180
        ceiling.transform.eulerAngles = new Vector3(0, 0, 180);

        return ceiling;
    }
 
    private static (GameObject roofPointLayer, GameObject roof, GameObject exportPackage) CreateContainerObj()
    {
        // generate roofPoints
        GameObject roofPointLayer = new GameObject("roofPoints");

        // create roof layer
        GameObject roof = new GameObject("roof");

        // create export package layer
        GameObject exportPackage = new GameObject("exportPackage");

        return (roofPointLayer, roof, exportPackage);
    }

    private static void DestroyContainerObj(GameObject item1, GameObject item2)
    {
        Destroy(item1);
        Destroy(item2);
    }

    private static List<GameObject> GetJoints(GameObject resultMesh)
    {
        List<GameObject> resultChildren = Utilities.GetChildren(resultMesh, filterName: "Spawn");
        List<GameObject> joints = Utilities.GetJoints(resultChildren, "checkPoint");
        return joints;
    }
}
