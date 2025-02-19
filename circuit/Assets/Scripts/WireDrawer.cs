using System.Collections.Generic;
using UnityEngine;

public class WireDrawer3D : MonoBehaviour
{
    public LineRenderer lineRenderer; // ���� �����
    public LayerMask connectionPointsLayer; // ���� ����� �������
    public Camera mainCamera; // �������� ��������

    private List<Vector3> wirePositions = new List<Vector3>();
    private bool isDrawing = false;

    void Update()
    {
        Vector3 mousePos = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0)) // ��� ����� ��� ������
        {
            if (!isDrawing) // ��� ��� �����
            {
                Collider hit = GetMouseHit();
                if (hit != null) // ���� �� ���� ����� �����
                {
                    isDrawing = true;
                    wirePositions.Clear();
                    wirePositions.Add(hit.transform.position); // ���� �������
                    wirePositions.Add(mousePos); // ���� ����� ���� ������

                    lineRenderer.positionCount = wirePositions.Count;
                    lineRenderer.SetPositions(wirePositions.ToArray());
                }
            }
            else // ����� ����� ��� ����� ��� ����
            {
                Collider hit = GetMouseHit();
                if (hit != null && hit.transform.position != wirePositions[0]) // ������ �� ���� ����� �����
                {
                    wirePositions[wirePositions.Count - 1] = hit.transform.position; // ����� ���� �������
                    lineRenderer.SetPositions(wirePositions.ToArray());
                }
                else
                {
                    // ��� �� ��� ������� ��� ����� �����
                    wirePositions.Clear();
                    lineRenderer.positionCount = 0;
                }

                isDrawing = false;
            }
        }

        // ����� ��� ���� ����� ������ ����� �����
        if (isDrawing && wirePositions.Count > 1)
        {
            wirePositions[wirePositions.Count - 1] = mousePos;
            lineRenderer.SetPositions(wirePositions.ToArray());
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point; // ����� ���� ������� �� ��������
        }
        return ray.GetPoint(10); // ��� �� ����� ����� �� ������ ������ ������
    }

    Collider GetMouseHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, connectionPointsLayer))
        {
            return hit.collider; // ����� ������ ������� ��
        }
        return null;
    }
}
