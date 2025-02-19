using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShowFaceNumbers : MonoBehaviour
{
    public MeshFilter meshFilter;

    void OnDrawGizmos()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (meshFilter != null && meshFilter.mesh != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                // «·Õ’Ê· ⁄·Ï «·‰ﬁ«ÿ «·À·«À… ··ÊÃÂ (Face)
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                //  ÕÊÌ· «·‰ﬁ«ÿ „‰ «·„”«Õ… «·„Õ·Ì… ≈·Ï «·„”«Õ… «·⁄«·„Ì…
                Vector3 worldV1 = transform.TransformPoint(v1);
                Vector3 worldV2 = transform.TransformPoint(v2);
                Vector3 worldV3 = transform.TransformPoint(v3);

                // Õ”«» «·„ ÃÂ «·⁄„ÊœÌ ⁄·Ï «·ÊÃÂ (Normal)
                Vector3 faceNormal = Vector3.Cross(worldV2 - worldV1, worldV3 - worldV1).normalized;

                // «· Õﬁﬁ „„« ≈–« ﬂ«‰ «·ÊÃÂ „ÊÃÂ« ‰ÕÊ Y+
                if (faceNormal.y > 0)
                {
                    // Õ”«» „—ﬂ“ «·ÊÃÂ
                    Vector3 faceCenter = (worldV1 + worldV2 + worldV3) / 3f;

                    // —”„ —ﬁ„ «·ÊÃÂ ›Ì „—ﬂ“Â
#if UNITY_EDITOR
                    Handles.Label(faceCenter, "Face " + (i / 3));
#endif

                    // —”„ ÕÊ«› «·„À·À »«” Œœ«„ Gizmos
                    Gizmos.color = Color.green; // ·Ê‰ «·ŒÿÊÿ
                    Gizmos.DrawLine(worldV1, worldV2); // «·Œÿ »Ì‰ «·‰ﬁÿ… 1 Ê«·‰ﬁÿ… 2
                    Gizmos.DrawLine(worldV2, worldV3); // «·Œÿ »Ì‰ «·‰ﬁÿ… 2 Ê«·‰ﬁÿ… 3
                    Gizmos.DrawLine(worldV3, worldV1); // «·Œÿ »Ì‰ «·‰ﬁÿ… 3 Ê«·‰ﬁÿ… 1
                }
            }
        }
    }
}