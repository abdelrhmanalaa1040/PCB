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
                // ������ ��� ������ ������� ����� (Face)
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                // ����� ������ �� ������� ������� ��� ������� ��������
                Vector3 worldV1 = transform.TransformPoint(v1);
                Vector3 worldV2 = transform.TransformPoint(v2);
                Vector3 worldV3 = transform.TransformPoint(v3);

                // ���� ������ ������� ��� ����� (Normal)
                Vector3 faceNormal = Vector3.Cross(worldV2 - worldV1, worldV3 - worldV1).normalized;

                // ������ ��� ��� ��� ����� ������ ��� Y+
                if (faceNormal.y > 0)
                {
                    // ���� ���� �����
                    Vector3 faceCenter = (worldV1 + worldV2 + worldV3) / 3f;

                    // ��� ��� ����� �� �����
#if UNITY_EDITOR
                    Handles.Label(faceCenter, "Face " + (i / 3));
#endif

                    // ��� ���� ������ �������� Gizmos
                    Gizmos.color = Color.green; // ��� ������
                    Gizmos.DrawLine(worldV1, worldV2); // ���� ��� ������ 1 ������� 2
                    Gizmos.DrawLine(worldV2, worldV3); // ���� ��� ������ 2 ������� 3
                    Gizmos.DrawLine(worldV3, worldV1); // ���� ��� ������ 3 ������� 1
                }
            }
        }
    }
}