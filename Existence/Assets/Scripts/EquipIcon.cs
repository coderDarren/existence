using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipIcon : MonoBehaviour
{
    public Texture2D itemIcon;
    public Texture2D[] specialIcon;    
    public enum Specials{quickSlice, chargeShot}
    public Specials[] specials;
    public GameObject skillSlot;
    public GameObject qsPrefab;
    public GameObject csPrefab;

    private PlayerCombatController m_CombatController; 
    private string m_ActiveSpecial;
    private bool m_Activated;

    public void SendSpecial(int _specialInt){ 
        m_ActiveSpecial = specials[_specialInt].ToString();        
        m_CombatController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        m_CombatController.SpecialAttack(m_ActiveSpecial);

    }

    private void Update(){
        if(!m_Activated){
            if(specials.Length > 0){
                if(specials[0].ToString() == ("quickSlice")){
                    m_Activated = true;
                    Instantiate(qsPrefab, GameObject.Find("slotOne").transform);
                }
                if(specials[0].ToString() == ("chargeShot")){
                    m_Activated = true;
                    Instantiate(csPrefab, GameObject.Find("slotOne").transform);
                
                }
            }
        }
    }
}
