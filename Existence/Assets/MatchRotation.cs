using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    
    void Update()
    {
        transform.rotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;
    }
}
