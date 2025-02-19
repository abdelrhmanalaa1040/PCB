using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICGenerator : MonoBehaviour
{
    public GameObject IC;
    public GameObject StartIC;
    public GameObject EndIC;
    public int IC2PinsNum;
    public float offset;
    public List<GameObject> ICPins;

    void Start()
    {
        GeneratIC();
    }

    void Update()
    {
        
    }

    void GeneratIC()
    {
        GameObject _IC = Instantiate(StartIC, transform.position, transform.rotation);
        _IC.transform.parent = transform;
        ICPins.Add(_IC);

        for (int i = 1; i < IC2PinsNum - 1; i++)
        {
            _IC = Instantiate(IC, transform.position + new Vector3(0, 0, offset * i), transform.rotation);
            _IC.transform.parent = transform;
            ICPins.Add(_IC);
        }

        _IC = Instantiate(EndIC, transform.position + new Vector3(0, 0, (IC2PinsNum - 1) * offset), transform.rotation);
        _IC.transform.parent = transform;
        ICPins.Add(_IC);
    }
}
