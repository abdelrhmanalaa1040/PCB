using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Build.Content;
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
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
      
        if (Input.GetMouseButton(0))
        {
            if (hit.collider != null )
            {
                GameObject _hole = hit.collider.gameObject;
                GameObject newWire = Instantiate(WirePrefab, _hole.transform.position, transform.rotation);
                WireGenerator = newWire.GetComponent<WireGenerator>();
                WirePoint = WireGenerator.points[WireGenerator.points.Count].gameObject;
                print("done");
            }
            else
            {
                print(hit.collider.gameObject.name);
            }
        }

        if (WirePoint != null)
        {
            MouseTracking(WirePoint);
        }
    }

    void MouseTracking(GameObject point)
    {
        point.transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
