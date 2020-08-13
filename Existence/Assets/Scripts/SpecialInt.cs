using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialInt : MonoBehaviour
{
    public void SendInt(){
        Session.instance.player.GetComponent<PlayerCombatController>().SpecialAttack();
    }
}
