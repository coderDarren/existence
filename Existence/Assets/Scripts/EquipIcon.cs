using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipIcon : MonoBehaviour
{
    public Texture2D itemIcon;
    public Texture2D[] specialIcon;    
    public enum Specials{quickSlice, chargeShot}
    public Specials[] specials;

    private PlayerCombatController m_CombatController; 
    private string m_ActiveSpecial;

    public void SendSpecial(int _specialInt){ 
        m_ActiveSpecial = specials[_specialInt].ToString();        
        m_CombatController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        m_CombatController.SpecialAttack(m_ActiveSpecial);

    }
}
