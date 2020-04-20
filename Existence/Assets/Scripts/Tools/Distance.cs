using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour
{
    public GameObject target;
    float distance;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        Debug.Log(distance);
    }
}
