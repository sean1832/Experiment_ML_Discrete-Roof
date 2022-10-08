using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProMeshUtilities : MonoBehaviour
{
    #region highlevel functions

    // spawn gameObjects at vertices locations
    public static void GenerateObjAtVertices(List<Vector3> vertices, GameObject parent = null)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject newObj = new GameObject($"RoofPoint_{i}");
            newObj.AddComponent<SphereCollider>().radius = 0.2f;
            newObj.transform.position = vertices[i];
            if (parent != null)
            {
                newObj.transform.parent = parent.transform;
            }

        }
    }

    // offset vertices at direction and returns a offset list of vertices
    public static List<Vector3> GetOffsetVertices(List<Vector3> vertices, Vector3 direction, float offsetDistance)
    {
        Vector3 offsetFactor = direction * offsetDistance;

        List<Vector3> movedVertices = new List<Vector3>();
        foreach (var vertex in vertices)
        {
            Vector3 movedVertex = vertex + offsetFactor;
            movedVertices.Add(movedVertex);
        }

        return movedVertices;
    }

    // get roof vertices by doing a raycast on the ceiling. if hits ceiling, return.
    public static List<Vector3> GetRaycastCeilingVert(List<Vector3> vertices, GameObject ceiling)
    {
        List<Vector3> hitVertices = new List<Vector3>();

        foreach (Vector3 vertex in vertices)
        {
            var hitInfo = new RaycastHit();
            Ray ray = new Ray(vertex, Vector3.up);
            if (!Physics.Raycast(ray, out hitInfo, 20f)) continue;
            if (hitInfo.collider.gameObject == ceiling)
            {
                hitVertices.Add(vertex);
            }
        }
        return hitVertices;
    }

    public static List<Vector3> GetProjectedVertices(GameObject resultMesh, string projectionPlane, float projectDistanceMultiplier, string spawnLayerName, string checkPtName)
    {
        // set local fields
        List<GameObject> resultChildren = Utilities.GetChildren(resultMesh, filterName: spawnLayerName);
        List<GameObject> joints = Utilities.GetJoints(resultChildren, checkPtName);

        float projectDistance;

        switch (projectionPlane)
        {
            case "x":
                projectDistance = Utilities.GetBounds(joints, "min").x;
                break;
            case "y":
                projectDistance = Utilities.GetBounds(joints, "min").y;
                break;
            case "z":
                projectDistance = Utilities.GetBounds(joints, "min").z;
                break;
            default: throw new ArgumentOutOfRangeException($"Projection Plane ({projectionPlane}) not accepted, please enter a valid plane. Plane option: x, y, z");
        }

        projectDistance *= projectDistanceMultiplier;

        // add joint positions to vertices
        List<Vector3> vertices = new List<Vector3>();
        foreach (GameObject joint in joints)
        {
            vertices.Add(joint.transform.position);
        }

        // project vertices on plane
        vertices = ProjectVertices(vertices, projectionPlane, projectDistance);
        vertices = Utilities.CullDuplicate(vertices);

        return vertices;
    }

    #endregion

    #region local functions

    private static List<Vector3> ProjectVertices(List<Vector3> vertices, string plane, float offsetDistance)
    {
        List<Vector3> flattenVertices = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = plane switch
            {
                "x" => new Vector3(offsetDistance, vertices[i].y, vertices[i].z),
                "z" => new Vector3(vertices[i].x, vertices[i].y, offsetDistance),
                _ => vertices[i]
            };
            flattenVertices.Add(v);
        }

        return flattenVertices;
    }

    #endregion
}
