using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WireGenerator : MonoBehaviour
{
    public List<Transform> points;
    public int curveResolution = 20;
    public int tubeResolution = 8;
    public float tubeRadius = 0.2f;

    private Mesh mesh;
    private List<Vector3> verticesList;
    private List<int> trianglesList;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateFullTube();
    }

    void Update()
    {
        if (HasPointsChanged())
        {
            GenerateFullTube();
        }
    }

    void GenerateFullTube()
    {
        if (points.Count < 3) return;

        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        for (int i = 0; i < points.Count - 2; i += 2)
        {
            GenerateTube(points[i], points[i + 1], points[i + 2]);
        }

        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
    }

    void GenerateTube(Transform _point0, Transform _point1, Transform _point2)
    {
        if (_point0 == null || _point1 == null || _point2 == null) return;

        int baseIndex = verticesList.Count;

        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 Direction1 = (_point0.localPosition - _point1.localPosition).normalized * tubeRadius;
            Vector3 Direction2 = (_point2.localPosition - _point1.localPosition).normalized * tubeRadius;

            Vector3 center = CalculateBezierPoint(t, Direction1, _point1.localPosition, Direction2);
            Vector3 tangent = CalculateBezierTangent(t, Direction1, _point1.localPosition, Direction2);
            Vector3 averageDirection = (Direction1 + Direction2).normalized;
            Vector3 normal = Vector3.Cross(tangent, averageDirection).normalized;
            Vector3 binormal = Vector3.Cross(tangent, normal).normalized;

            for (int j = 0; j < tubeResolution; j++)
            {
                float angle = (j / (float)tubeResolution) * Mathf.PI * 2;
                Vector3 offset = (Mathf.Cos(angle) * normal + Mathf.Sin(angle) * binormal) * tubeRadius;
                verticesList.Add(center + offset);
            }
        }

        for (int i = 0; i < curveResolution; i++)
        {
            for (int j = 0; j < tubeResolution; j++)
            {
                int current = baseIndex + i * tubeResolution + j;
                int next = baseIndex + i * tubeResolution + (j + 1) % tubeResolution;
                int nextRow = baseIndex + (i + 1) * tubeResolution + j;
                int nextRowNext = baseIndex + (i + 1) * tubeResolution + (j + 1) % tubeResolution;

                trianglesList.Add(current);
                trianglesList.Add(next);
                trianglesList.Add(nextRow);

                trianglesList.Add(next);
                trianglesList.Add(nextRowNext);
                trianglesList.Add(nextRow);
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }

    Vector3 CalculateBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1)).normalized;
    }

    bool HasPointsChanged()
    {
        foreach (var point in points)
        {
            if (point.hasChanged)
            {
                point.hasChanged = false;
                return true;
            }
        }
        return false;
    }
}
