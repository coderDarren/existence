using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnim : MonoBehaviour
{
    public float speed;

       
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speed, Space.World);
    }
}
