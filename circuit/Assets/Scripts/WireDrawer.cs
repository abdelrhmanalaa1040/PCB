using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireDrawer : MonoBehaviour
{
    public WireGenerator WireGenerator;
    public GameObject WirePrefab;
    public Camera cam;
    public GameObject WirePoint;

    void Start()
    {

    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                GameObject _hole = hit.collider.gameObject;

                GameObject newWire = Instantiate(WirePrefab, _hole.transform.position, Quaternion.identity);
                WireGenerator = newWire.GetComponent<WireGenerator>();

                if (WireGenerator.points.Count > 0)
                {
                    WirePoint = WireGenerator.points[WireGenerator.points.Count - 1].gameObject;
                }
            }
        }

        if (WirePoint != null)
        {
            MouseTracking(WirePoint);
        }
    }

    void MouseTracking(GameObject point)
    {
        Vector3 mousePos = Input.mousePosition;
      //  mousePos.z = 0; 
        point.transform.position = cam.ScreenToWorldPoint(mousePos);
    }
}