using System.Collections.Generic;
using UnityEngine;

public class SolderingBoardGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float offset = 1f;
    public GameObject solderingBoard;
    List<List<GameObject>> points; 

    void Start()
    {
        GenerateSolderingBoard();
    }

    void GenerateSolderingBoard()
    {
        points = new List<List<GameObject>>();

        for (int i = 0; i < width; i++)
        {
            points.Add(new List<GameObject>()); 

            for (int j = 0; j < height; j++)
            {
                GameObject _point = Instantiate(solderingBoard, transform.position + new Vector3(j * offset, 0, i * offset), transform.rotation);
                _point.transform.parent = transform;
                points[i].Add(_point); 
            }
        }
    }

}