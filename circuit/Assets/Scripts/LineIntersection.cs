using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cuboid
{
    public List<Vector3> points = new List<Vector3>(8);
}

public class LineIntersection : MonoBehaviour
{
    public Vector3 point1 = new Vector3(1, 1, 1);
    public Vector3 point2 = new Vector3(7, 7, 7);

    public List<Cuboid> cuboids = new List<Cuboid>();

    void Start()
    {
        if (cuboids.Count == 0)
        {
            AddCuboid(new Vector3(2, 2, 2), new Vector3(4, 2, 2), new Vector3(4, 4, 2), new Vector3(2, 4, 2),
                      new Vector3(2, 2, 4), new Vector3(4, 2, 4), new Vector3(4, 4, 4), new Vector3(2, 4, 4));
        }
    }

    public void AddCuboid(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5, Vector3 p6, Vector3 p7, Vector3 p8)
    {
        cuboids.Add(new Cuboid { points = new List<Vector3> { p1, p2, p3, p4, p5, p6, p7, p8 } });
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(point1, point2);

        foreach (var cuboid in cuboids)
        {
            if (cuboid.points.Count < 8) continue;

            Gizmos.color = Color.black;

            // —”„ ÕœÊœ «·„ﬂ⁄»
            DrawCuboidEdges(cuboid.points);

            // ›Õ’ «· ﬁ«ÿ⁄ „⁄ «·„ﬂ⁄»
            if (LineIntersectsCuboid(point1, point2, cuboid.points))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((point1 + point2) / 2, 0.2f);
            }
        }
    }

    void DrawCuboidEdges(List<Vector3> points)
    {
        int[,] edges = new int[,]
        {
            {0,1}, {1,2}, {2,3}, {3,0},
            {4,5}, {5,6}, {6,7}, {7,4},
            {0,4}, {1,5}, {2,6}, {3,7}
        };

        for (int i = 0; i < edges.GetLength(0); i++)
        {
            Gizmos.DrawLine(points[edges[i, 0]], points[edges[i, 1]]);
        }
    }

    bool LineIntersectsCuboid(Vector3 p1, Vector3 p2, List<Vector3> points)
    {
        // Õ”«» Bounding Box ··„ﬂ⁄»
        Bounds bounds = GetBounds(points);

        // «Œ »«— «· ﬁ«ÿ⁄ „⁄ Bounding Box ﬂŒÿÊ… √Ê·Ì…
        if (!bounds.IntersectRay(new Ray(p1, (p2 - p1).normalized)))
            return false;

        //  ⁄—Ì› «·√ÊÃÂ (ﬂ· ÊÃÂ Ì „  ⁄—Ì›Â »À·«À ‰ﬁ«ÿ · ‘ﬂÌ· „” ÊÏ)
        Vector3[,] faces = new Vector3[,]
        {
            { points[0], points[1], points[2] }, { points[0], points[2], points[3] }, // Face 1
            { points[4], points[5], points[6] }, { points[4], points[6], points[7] }, // Face 2
            { points[0], points[1], points[5] }, { points[0], points[5], points[4] }, // Face 3
            { points[2], points[3], points[7] }, { points[2], points[7], points[6] }, // Face 4
            { points[0], points[3], points[7] }, { points[0], points[7], points[4] }, // Face 5
            { points[1], points[2], points[6] }, { points[1], points[6], points[5] }  // Face 6
        };

        for (int i = 0; i < faces.GetLength(0); i++)
        {
            if (LineIntersectsTriangle(p1, p2, faces[i, 0], faces[i, 1], faces[i, 2]))
                return true;
        }

        return false;
    }

    Bounds GetBounds(List<Vector3> points)
    {
        Vector3 min = points[0], max = points[0];
        foreach (var p in points)
        {
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }
        return new Bounds((min + max) / 2, max - min);
    }

    bool LineIntersectsTriangle(Vector3 p1, Vector3 p2, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0, edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(p2 - p1, edge2);
        float a = Vector3.Dot(edge1, h);

        if (Mathf.Abs(a) < 1e-5f) return false;

        float f = 1.0f / a;
        Vector3 s = p1 - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0 || u > 1.0) return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(p2 - p1, q);

        if (v < 0.0 || u + v > 1.0) return false;

        float t = f * Vector3.Dot(edge2, q);

        return t > 0 && t < 1;
    }
}
