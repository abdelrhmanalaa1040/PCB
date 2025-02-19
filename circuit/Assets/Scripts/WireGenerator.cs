using UnityEngine;
using System;
using Unity.Mathematics;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WireGenerator : MonoBehaviour
{
    public int segments = 32; // ÚÏÏ ÇáÃÌÒÇÁ Íæá ãÍæÑ ÇáÃÓØæÇäÉ (íÒíÏ ßËÇİÉ ÇáãËáËÇÊ)
    public float radius = 0.5f; // äÕİ ŞØÑ ÇáÃÓØæÇäÉ (ÇáŞØÑ = 1)
    public Material material; // ÇáãÇÏÉ (Material) ÇáãØÈŞÉ Úáì ÇáÃÓØæÇäÉ
    public int numberOfPoints = 3; // ÚÏÏ ÇáäŞÇØ
    public GameObject pointPrefab; // ÈÑíİÇÈ ááäŞÇØ (GameObject)
    public int curveResolution = 20; // ÏŞÉ ÇáãäÍäì (ÚÏÏ ÇáäŞÇØ Èíä ßá äŞØÊíä)

    private GameObject[] points; // ãÕİæİÉ áÊÎÒíä ÇáäŞÇØ (GameObjects)
    private Mesh mesh;

    void Start()
    {
        InitializePoints();
        GenerateSmoothCylinder();
    }

    void Update()
    {
        // ÊÍÏíË ÇáÃÓØæÇäÉ İí ÇáæŞÊ ÇáİÚáí ÚäÏ ÊÛííÑ Ãí ÅÚÏÇÏ
        GenerateSmoothCylinder();
    }

    void OnValidate()
    {
        // ÅĞÇ Êã ÊÛííÑ ÚÏÏ ÇáäŞÇØ İí ÇáãİÊÇÍ (Inspector)¡ Şã ÈÊÍÏíË ÇáäŞÇØ
        if (points == null || numberOfPoints != points.Length)
        {
            InitializePoints();
        }
    }

    void InitializePoints()
    {
        // ÊäÙíİ ÇáäŞÇØ ÇáŞÏíãÉ ÅĞÇ ßÇäÊ ãæÌæÏÉ
        if (points != null)
        {
            foreach (var point in points)
            {
                if (point != null)
                {
                    if (Application.isPlaying)
                        Destroy(point); // ÍĞİ ÇáäŞÇØ İí æÖÚ ÇáÊÔÛíá
                    else
                        DestroyImmediate(point); // ÍĞİ ÇáäŞÇØ İí ÇáæÖÚ ÇáÊÍÑíÑí
                }
            }
        }

        // ÅäÔÇÁ Ãæ ÊÚÏíá ÇáäŞÇØ
        points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            string pointName = $"{gameObject.name}_Point_{i}"; // ÇÓã İÑíÏ ááäŞØÉ
            Transform existingPoint = transform.Find(pointName);

            if (existingPoint != null)
            {
                // ÅĞÇ ßÇäÊ ÇáäŞØÉ ãæÌæÏÉ ÈÇáİÚá¡ äÓÊÎÏãåÇ
                points[i] = existingPoint.gameObject;
            }
            else
            {
                // ÅĞÇ áã Êßä ÇáäŞØÉ ãæÌæÏÉ¡ ääÔÆ æÇÍÏÉ ÌÏíÏÉ
                points[i] = Instantiate(pointPrefab, transform); // ÇáäŞÇØ ÊÇÈÚÉ ááÜ Parent
                points[i].name = pointName;
                points[i].transform.localPosition = new Vector3(i * 2, 0, 0); // ÊæÒíÚ ÇáäŞÇØ Úáì ÇáãÍæÑ X

#if UNITY_EDITOR
                // ÊÍÏíÏ Ãä ÇáßÇÆä ŞÏ Êã ÊÚÏíáå æíÌÈ ÍİÙå
                EditorUtility.SetDirty(points[i]);
#endif
            }
        }

        // ÍĞİ ÇáäŞÇØ ÇáÒÇÆÏÉ ÅĞÇ ßÇä ÚÏÏ ÇáäŞÇØ ÇáÍÇáí ÃßÈÑ ãä ÇáãØáæÈ
        for (int i = numberOfPoints; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        // ÅäÔÇÁ ÇáÔÈßÉ (Mesh) ÇáÌÏíÏÉ
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    void GenerateSmoothCylinder()
    {
        if (mesh == null || points == null) return;

        // ÌãÚ ãæÇŞÚ ÇáäŞÇØ ÇáãÍáíÉ
        Vector3[] pointPositions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogWarning($"Point {i} is missing. Reinitializing points.");
                InitializePoints();
                return;
            }
            pointPositions[i] = points[i].transform.localPosition; // ÇÓÊÎÏÇã ÇáÅÍÏÇËíÇÊ ÇáãÍáíÉ
        }

        // ÅäÔÇÁ ÇáãäÍäì ÈÇÓÊÎÏÇã Bezier Curve
        Vector3[] curvePoints = CreateBezierCurve(pointPositions, curveResolution);

        int totalVertices = segments * curvePoints.Length;
        int totalTriangles = segments * (curvePoints.Length - 1) * 6;

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[totalTriangles];
        Vector2[] uv = new Vector2[totalVertices];

        int vertexIndex = 0;
        int triangleIndex = 0;

        // ÅäÔÇÁ ÇáŞÇÚÏÉ ÇáÚáæíÉ æÇáÓİáíÉ áßá ÌÒÁ ãä ÇáãäÍäì
        for (int p = 0; p < curvePoints.Length; p++)
        {
            Vector3 currentPoint = curvePoints[p];
            Vector3 direction = p == 0 ? (curvePoints[p + 1] - currentPoint).normalized :
                              p == curvePoints.Length - 1 ? (currentPoint - curvePoints[p - 1]).normalized :
                              (curvePoints[p + 1] - curvePoints[p - 1]).normalized;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                int result = (p % curveResolution == 0) ? 1 : 0;
                
                Vector3 vertex =  rotation * new Vector3(x, radius + ((Vector3.Distance(currentPoint, direction) - (2 * radius) * ((p % curveResolution) / curveResolution)) ), z);
                vertices[vertexIndex] = vertex;

                uv[vertexIndex] = new Vector2((float)i / segments, (float)p / curvePoints.Length);
                vertexIndex++;
            }
        }

        // ÅäÔÇÁ ÇáãËáËÇÊ
        for (int p = 0; p < curvePoints.Length - 1; p++)
        {
            for (int i = 0; i < segments; i++)
            {
                int current = p * segments + i;
                int next = p * segments + (i + 1) % segments;
                int nextSegment = (p + 1) * segments + i;
                int nextSegmentNext = (p + 1) * segments + (i + 1) % segments;

                triangles[triangleIndex] = current;
                triangles[triangleIndex + 1] = nextSegment;
                triangles[triangleIndex + 2] = next;

                triangles[triangleIndex + 3] = next;
                triangles[triangleIndex + 4] = nextSegment;
                triangles[triangleIndex + 5] = nextSegmentNext;

                triangleIndex += 6;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        // ÊØÈíŞ ÇáãÇÏÉ (Material)
        GetComponent<MeshRenderer>().material = material;
    }

    // ÅäÔÇÁ ãäÍäì Bezier
    Vector3[] CreateBezierCurve(Vector3[] points, int resolution)
    {
        Vector3[] curve = new Vector3[(points.Length - 1) * resolution + 1];
        int index = 0;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[i + 1];
            Vector3 controlPoint0 = p0 + (p1 - p0) * 0.25f; // äŞØÉ ÊÍßã 1
            Vector3 controlPoint1 = p0 + (p1 - p0) * 0.75f; // äŞØÉ ÊÍßã 2

            for (int t = 0; t < resolution; t++)
            {
                float tNormalized = (float)t / resolution;
                curve[index] = CalculateBezierPoint(p0, controlPoint0, controlPoint1, p1, tNormalized);
                index++;
            }
        }
        curve[index] = points[points.Length - 1]; // ÅÖÇİÉ ÇáäŞØÉ ÇáÃÎíÑÉ

        return curve;
    }

    // ÍÓÇÈ äŞØÉ Úáì ãäÍäì Bezier
    Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0; // (1-t)^3 * P0
        point += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
        point += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
        point += ttt * p3; // t^3 * P3

        return point;
    }
}