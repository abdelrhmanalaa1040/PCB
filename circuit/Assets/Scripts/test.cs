using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class test : MonoBehaviour
{
    public Transform point0, point1, point2;
    public int curveResolution = 20;
    public int tubeResolution = 8;
    public float tubeRadius = 0.2f;

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateTube();
    }

    private void Update()
    {
        GenerateTube();
    }

    void GenerateTube()
    {
        if (point0 == null || point1 == null || point2 == null)
            return;

        Vector3[] vertices = new Vector3[(curveResolution + 1) * tubeResolution];
        int[] triangles = new int[curveResolution * tubeResolution * 6];

        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 center = CalculateBezierPoint(t, point0.position, point1.position, point2.position);
            Vector3 tangent = CalculateBezierTangent(t, point0.position, point1.position, point2.position);
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            for (int j = 0; j < tubeResolution; j++)
            {
                float angle = (j / (float)tubeResolution) * Mathf.PI * 2;
                Vector3 offset = (Mathf.Cos(angle) * normal + Mathf.Sin(angle) * binormal) * tubeRadius;
                vertices[i * tubeResolution + j] = center + offset;
            }
        }

        int triIndex = 0;
        for (int i = 0; i < curveResolution; i++)
        {
            for (int j = 0; j < tubeResolution; j++)
            {
                int current = i * tubeResolution + j;
                int next = i * tubeResolution + (j + 1) % tubeResolution;
                int nextRow = (i + 1) * tubeResolution + j;
                int nextRowNext = (i + 1) * tubeResolution + (j + 1) % tubeResolution;

                triangles[triIndex++] = current;
                triangles[triIndex++] = next;
                triangles[triIndex++] = nextRow;

                triangles[triIndex++] = next;
                triangles[triIndex++] = nextRowNext;
                triangles[triIndex++] = nextRow;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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
}
