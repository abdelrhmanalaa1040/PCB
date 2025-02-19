using UnityEngine;

public class PipeIntersection : MonoBehaviour
{
    private Transform otherPipe;
    private bool isIntersecting = false;
    private Vector3 intersectionPoint; // ���� �������

    void Start()
    {
        // ������ ��� ������� ������ ��������
        GameObject otherPipeObject = GameObject.FindWithTag("Pipe");
        if (otherPipeObject != null && otherPipeObject.transform != transform)
        {
            otherPipe = otherPipeObject.transform;
        }
        else
        {
            Debug.LogError(" �� ��� ������ ��� ������� ������! ���� �� ����� Tag ���� 'Pipe'");
        }
    }

    void Update()
    {
        if (otherPipe == null) return; // ���� ����� ��� �� ��� ������ ��� �������

        if (CheckIntersection(transform, otherPipe, out intersectionPoint))
        {
            if (!isIntersecting)
            {
                Debug.Log($" ��������� ��������� ��� ������: {intersectionPoint}");
                isIntersecting = true;
            }

            // ��� ���� ������� �� ������
            Debug.DrawRay(intersectionPoint, Vector3.up * 0.5f, Color.red);
        }
        else
        {
            if (isIntersecting)
            {
                Debug.Log(" ��������� �� ����� ���������.");
                isIntersecting = false;
            }
        }
    }

    bool CheckIntersection(Transform pipe1, Transform pipe2, out Vector3 intersection)
    {
        Bounds bounds1 = GetMeshBounds(pipe1);
        Bounds bounds2 = GetMeshBounds(pipe2);

        if (bounds1.Intersects(bounds2))
        {
            // ���� ���� ������� (������� ����� ��� ���� �� �����)
            intersection = (bounds1.center + bounds2.center) / 2;
            return true;
        }

        intersection = Vector3.zero;
        return false;
    }

    Bounds GetMeshBounds(Transform obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null) return new Bounds(obj.position, Vector3.zero);

        Mesh mesh = meshFilter.mesh;
        Bounds bounds = mesh.bounds;
        bounds.center = obj.position;
        bounds.extents = Vector3.Scale(bounds.extents, obj.lossyScale);
        return bounds;
    }
}
