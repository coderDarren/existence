
using UnityEngine;
using System.Threading.Tasks;

public class CombatTester : GameSystem
{
    public KeyCode damageTestKey;
    public GameObject player;
    
      

    private bool attacking;
    private Animator anim;
    
#region Unity Functions

    private void Start(){
        
        
    }

    private async void Update() {
        anim = player.GetComponent<Animator>();    

        
                     
    }
#endregion
}

