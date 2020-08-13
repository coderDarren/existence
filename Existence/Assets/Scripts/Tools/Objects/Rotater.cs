using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 direction;

    void Update()
    {
        transform.Rotate(direction * Time.deltaTime, Space.World);
    }
}
