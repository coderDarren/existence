
using UnityEngine;
using System.Threading.Tasks;

public class CombatTester : GameSystem
{
    public KeyCode damageTestKey;
    public GameObject player;
    public string weapon;  

    private bool attacking;
    private Animator anim;
    
#region Unity Functions

    private void Start(){
        attacking = false;
    }

    private async void Update() {
        anim = player.GetComponent<Animator>();    

        
        if (!GameObject.FindGameObjectWithTag("CombatTestDummy")) return;
        if (Input.GetKeyDown(damageTestKey)) {
            if(!attacking){
                attacking = true;
                anim.SetBool("attacking"+ weapon, true);                
            }
            else {
                attacking = false;
                anim.SetBool("attacking"+ weapon, false);              
            }
        }        
    }
#endregion
}

