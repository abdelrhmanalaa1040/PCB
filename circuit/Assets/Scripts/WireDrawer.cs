using System.Collections.Generic;
using UnityEngine;

public class WireDrawer3D : MonoBehaviour
{
    public LineRenderer lineRenderer; // áÑÓã ÇáÓáß
    public LayerMask connectionPointsLayer; // ØÈŞÉ áäŞÇØ ÇáÊæÕíá
    public Camera mainCamera; // ÇáßÇãíÑÇ ÇáÑÆíÓíÉ

    private List<Vector3> wirePositions = new List<Vector3>();
    private bool isDrawing = false;

    void Update()
    {
        Vector3 mousePos = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0)) // ÚäÏ ÇáÖÛØ Úáì ÇáãÇæÓ
        {
            if (!isDrawing) // ÈÏÁ ÑÓã ÇáÓáß
            {
                Collider hit = GetMouseHit();
                if (hit != null) // ÊÍŞŞ ãä äŞØÉ ÈÏÇíÉ ÕÍíÍÉ
                {
                    isDrawing = true;
                    wirePositions.Clear();
                    wirePositions.Add(hit.transform.position); // äŞØÉ ÇáÈÏÇíÉ
                    wirePositions.Add(mousePos); // äŞØÉ ãÄŞÊÉ ÊÊÈÚ ÇáãÇæÓ

                    lineRenderer.positionCount = wirePositions.Count;
                    lineRenderer.SetPositions(wirePositions.ToArray());
                }
            }
            else // ÅäåÇÁ ÇáÓáß ÚäÏ ÇáÖÛØ ãÑÉ ÃÎÑì
            {
                Collider hit = GetMouseHit();
                if (hit != null && hit.transform.position != wirePositions[0]) // ÇáÊÃßÏ ãä äŞØÉ äåÇíÉ ÕÍíÍÉ
                {
                    wirePositions[wirePositions.Count - 1] = hit.transform.position; // ÊËÈíÊ äŞØÉ ÇáäåÇíÉ
                    lineRenderer.SetPositions(wirePositions.ToArray());
                }
                else
                {
                    // ÅĞÇ áã íÊã ÇáÊæÕíá¡ íÊã ÅáÛÇÁ ÇáÓáß
                    wirePositions.Clear();
                    lineRenderer.positionCount = 0;
                }

                isDrawing = false;
            }
        }

        // ÊÍÏíË ÂÎÑ äŞØÉ áÊÊÈÚ ÇáãÇæÓ ÃËäÇÁ ÇáÑÓã
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
            return hit.point; // ÅÑÌÇÚ ãæÖÚ ÇáÊÕÇÏã ãÚ ÇáãÌÓãÇÊ
        }
        return ray.GetPoint(10); // ÅĞÇ áã íÕØÏã ÈÔíÁ¡ ÖÚ ÇáäŞØÉ ÈÚíÏğÇ ŞáíáğÇ
    }

    Collider GetMouseHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, connectionPointsLayer))
        {
            return hit.collider; // ÅÑÌÇÚ ÇáãÌÓã ÇáãÕØÏã Èå
        }
        return null;
    }
}
