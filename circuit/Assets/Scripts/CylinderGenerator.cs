using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CylinderGenerator : MonoBehaviour
{
    public int segments = 32; // ⁄œœ «·√Ã“«¡ ÕÊ· „ÕÊ— «·√”ÿÊ«‰…
    public float radius = 0.5f; // ‰’› ﬁÿ— «·√”ÿÊ«‰…
    public Material material; // «·„«œ… «·„ÿ»ﬁ… ⁄·Ï «·√”ÿÊ«‰…
    public int numberOfPoints = 3; // ⁄œœ «·‰ﬁ«ÿ
    public GameObject pointPrefab; // »—Ì›«» ··‰ﬁ«ÿ

    private GameObject[] points; // „’›Ê›… · Œ“Ì‰ «·‰ﬁ«ÿ
    private Mesh mesh;

    void Start()
    {
        InitializePoints();
        GenerateIndependentCylinders();
    }

    void Update()
    {
        GenerateIndependentCylinders();
    }

    void OnValidate()
    {
        if (points == null || numberOfPoints != points.Length)
        {
            InitializePoints();
        }
    }

    void InitializePoints()
    {
        if (points != null)
        {
            foreach (var point in points)
            {
                if (point != null)
                {
                    if (Application.isPlaying)
                        Destroy(point);
                    else
                        DestroyImmediate(point);
                }
            }
        }

        points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            string pointName = $"{gameObject.name}_Point_{i}";
            Transform existingPoint = transform.Find(pointName);

            if (existingPoint != null)
            {
                points[i] = existingPoint.gameObject;
            }
            else
            {
                points[i] = Instantiate(pointPrefab, transform);
                points[i].name = pointName;
                points[i].transform.localPosition = new Vector3(i * 2, 0, 0);

#if UNITY_EDITOR
                EditorUtility.SetDirty(points[i]);
#endif
            }
        }

        for (int i = numberOfPoints; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    void GenerateIndependentCylinders()
    {
        if (mesh == null || points == null || points.Length < 2) return;

        Vector3[] pointPositions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogWarning($"Point {i} is missing. Reinitializing points.");
                InitializePoints();
                return;
            }
            pointPositions[i] = points[i].transform.localPosition;
        }

        int totalVertices = segments * 2 * (points.Length - 1);
        int totalTriangles = segments * (points.Length - 1) * 6;

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[totalTriangles];
        Vector2[] uv = new Vector2[totalVertices];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int p = 0; p < points.Length - 1; p++)
        {
            Vector3 startPoint = pointPositions[p];
            Vector3 endPoint = pointPositions[p + 1];
            Vector3 direction = (endPoint - startPoint).normalized;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                if (p == points.Length - 1)
                {
                    vertices[vertexIndex] = startPoint + rotation * new Vector3(x, 0, z);
                    uv[vertexIndex] = new Vector2((float)i / segments, 0);
                    vertexIndex++;

                    vertices[vertexIndex] = endPoint + rotation * new Vector3(x, 0, z) + new Vector3(-radius, 0, 0);
                    uv[vertexIndex] = new Vector2((float)i / segments, 1);
                    vertexIndex++;
                }
                else if (p == points.Length - 1)
                {
                    vertices[vertexIndex] = startPoint + rotation * new Vector3(x, 0, z) + new Vector3(radius, 0, 0);
                    uv[vertexIndex] = new Vector2((float)i / segments, 0);
                    vertexIndex++;

                    vertices[vertexIndex] = endPoint + rotation * new Vector3(x, 0, z);
                    uv[vertexIndex] = new Vector2((float)i / segments, 1);
                    vertexIndex++;
                    print(p);
                }
                else
                {
                    vertices[vertexIndex] = startPoint + rotation * new Vector3(x, 0, z) + new Vector3(0, 0, 0);
                    uv[vertexIndex] = new Vector2((float)i / segments, 0);
                    vertexIndex++;

                    vertices[vertexIndex] = endPoint + rotation * new Vector3(x, 0, z) + new Vector3(0, 0, 0);
                    uv[vertexIndex] = new Vector2((float)i / segments, 1);
                    vertexIndex++;
                }
            }

            for (int i = 0; i < segments; i++)
            {
                int current = p * segments * 2 + i * 2;
                int next = p * segments * 2 + ((i + 1) % segments) * 2;

                // ⁄ﬂ”  — Ì» «·›Ì— ﬂ” ·Ã⁄· «·”ÿÕ «·œ«Œ·Ì ÂÊ «·„—∆Ì
                triangles[triangleIndex] = current;
                triangles[triangleIndex + 1] = current + 1;
                triangles[triangleIndex + 2] = next + 1;

                triangles[triangleIndex + 3] = current;
                triangles[triangleIndex + 4] = next + 1;
                triangles[triangleIndex + 5] = next;

                triangleIndex += 6;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        GetComponent<MeshRenderer>().material = material;
    }
}
