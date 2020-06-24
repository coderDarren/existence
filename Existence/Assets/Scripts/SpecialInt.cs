using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialInt : MonoBehaviour
{
    public int specialInt;

    public void SendInt(){
        GameObject.FindGameObjectWithTag("Weapon").GetComponent<EquipIcon>().SendSpecial(specialInt);
  }
}
