using System;
using System.Collections.Generic;
using UnityEngine;

public class ProMeshUtilities : MonoBehaviour
{
    #region public functions

    // spawn gameObjects at vertices locations
    public static void GenerateColliderAtVertices(List<Vector3> vertices, GameObject parent = null)
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
            if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) continue;
            if (hitInfo.collider.gameObject == ceiling)
            {
                hitVertices.Add(vertex);
            }
        }
        return hitVertices;
    }

    public static List<Vector3> GetProjectedVertices(GameObject resultMesh, string projectionPlane, float overhangDistance, string checkPtName)
    {
        // set local fields
        List<GameObject> resultChildren = Utilities.GetChildren(resultMesh);
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

        projectDistance -= overhangDistance;

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

    public static GameObject CombineMeshes(List<GameObject> sourceObj, GameObject targetObj)
    {
        var combine = new CombineInstance[sourceObj.Count];

        for (int i = 0; i < sourceObj.Count; i++)
        {
            MeshFilter sourceFilter = sourceObj[i].GetComponent<MeshFilter>();
            combine[i].mesh = sourceFilter.sharedMesh;
            combine[i].transform = sourceFilter.transform.localToWorldMatrix;
        }
        var mesh = new Mesh();
        mesh.CombineMeshes(combine);

        targetObj.AddComponent<MeshFilter>().mesh = mesh;
        targetObj.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        targetObj.GetComponent<MeshFilter>().mesh.name = "Procedural Roof Mesh";

        return targetObj;
    }


    public static Mesh GetMesh(GameObject obj)
    {
        if (obj.TryGetComponent(out MeshFilter filter))
        {
            return filter.mesh;
        }
        else
        {
            throw new ArgumentException(
                $"input gameObject: {obj.name} does not have a meshFilter, please apply one before proceed");
        }
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
