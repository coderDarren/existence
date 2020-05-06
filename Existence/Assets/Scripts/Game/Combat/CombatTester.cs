
using UnityEngine;
using System.Threading.Tasks;

public class CombatTester : GameSystem
{
    public KeyCode damageTestKey;
    public GameObject player;
    public string weapon;
    
    private Animator anim;
    private ParticleSystem[] particles;
    private bool attacking;
    private float timer;
    

#region Unity Functions

    private void Start(){
        attacking = false;
    }

    private async void Update() {
        anim = player.GetComponent<Animator>();
        particles = player.GetComponentsInChildren<ParticleSystem>();
        timer += Time.deltaTime;
        
        if (!GameObject.FindGameObjectWithTag("CombatTestDummy")) return;
        if (Input.GetKeyDown(damageTestKey)) {
            if(!attacking){
                attacking = true;
                anim.SetBool("attacking"+ weapon, true);
                /*foreach(ParticleSystem particle in particles){
                    particle.Play();                 
                }          */      
                timer = 1;
            }
            else {
                attacking = false;
                anim.SetBool("attacking"+ weapon, false);
               /* foreach(ParticleSystem particle in particles){
                    particle.Stop();
                }*/
                
            }
        }
        if(attacking){    
           
            if(timer >= 2){
                //GameObject.FindGameObjectWithTag("CombatTestDummy").GetComponent<Mob>().Hit(50);
                
                timer = 0;
            }
        }
    }
#endregion
}
