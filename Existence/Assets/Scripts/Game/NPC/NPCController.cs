using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class NPCController : MonoBehaviour
{
    public float runSpeed = 6.0f;
    public float strafeSpeed = 3.0f;
    public float backwardsSpeed = 1.0f;
    public float turnSpeed = 1.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Vector3[] rallyPoint;
    public int rallyDelay;    
    public GameObject target;

    private CharacterController m_Controller;
    private Animator m_Animator;
    private Material m_Material;
    private Vector3 m_MoveDirection = Vector3.zero;
    private float m_ForwardInput;
    private float m_TurnInput;
    private float m_StrafeInput;
    private float timer;
    private bool m_JumpInput;   
    private float distance;
    Quaternion talkRotation;
    Quaternion lookRotation;
    Quaternion currentRotation;
    Vector3 direction;
    Vector3 currentPos;
    Vector3 dirVector;
    Vector3 playerDir;
    Vector3 playerPos;    
    int currentRally;
    GameObject player;
    bool gotClicked;
    
    
    
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_ForwardInput = 0.2f;
        m_Material = target.GetComponent<Renderer>().material;
        
    }

    public void Update()
    {    
        currentPos = transform.position;
        currentRotation = transform.rotation;
        if(rallyPoint.Length > 0)
            dirVector = new Vector3(rallyPoint[currentRally].x, currentPos.y, rallyPoint[currentRally].z);
        distance = Vector3.Distance(currentPos, dirVector);      
        
        Pause();
        Move();
        Turn();
        ApproachZeroStrafe();
        Animate();
        
    }
    private void Move() {
        //if (m_Controller.isGrounded)
        //{
            float _forwardSpeed = m_ForwardInput < 0 ? backwardsSpeed : runSpeed;
            float _strafeSpeed = strafeSpeed;
            float _netSpeed = Mathf.Abs(m_ForwardInput) > 0 && Mathf.Abs(m_StrafeInput) > 0 ? (_forwardSpeed + _strafeSpeed) / 4.0f : 
                              Mathf.Abs(m_ForwardInput) > 0 ? _forwardSpeed : Mathf.Abs(m_StrafeInput) > 0 ? _strafeSpeed : 0.0f;
            m_MoveDirection = (transform.forward * m_ForwardInput + transform.right * m_StrafeInput) * _netSpeed;

            if (m_JumpInput)
            {
                m_MoveDirection.y = jumpSpeed;
            }
        //}
        m_MoveDirection.y -= gravity * Time.deltaTime;
        m_Controller.Move(m_MoveDirection * Time.deltaTime);
    }
    private void Turn() {         
        direction = dirVector - currentPos;
        lookRotation = Quaternion.LookRotation(direction);
        if(gotClicked){
            transform.rotation = Quaternion.Slerp(currentRotation, talkRotation, turnSpeed * Time.deltaTime);
        }
        else{
            transform.rotation = Quaternion.Slerp(currentRotation, lookRotation, turnSpeed * Time.deltaTime);         
        }
    }

    private float ApproachZeroStrafe() {
        m_StrafeInput = Mathf.Lerp(m_StrafeInput, 0, 5 * Time.deltaTime);
        if (Mathf.Abs(m_StrafeInput) <= 0.025f) {
            m_StrafeInput = 0;
        }
        return m_StrafeInput;
    }

    private void Animate() {
        m_Animator.SetFloat("running", m_ForwardInput);
        m_Animator.SetFloat("strafing", m_StrafeInput);
    }
    
    
    async void Pause() {
        if(distance <= 0.5f)
        {
            m_ForwardInput = 0.0f;
            turnSpeed = 0.0f;
            m_Material.SetFloat("_movingBool", 0f);
              
           
           if(currentRally != rallyPoint.Length - 1) {            
            currentRally ++;
            }

            else{
            System.Array.Reverse(rallyPoint);
            currentRally = 0;
            }  
            
            await Task.Delay(rallyDelay);
            m_ForwardInput = 0.2f;
            turnSpeed = 1.0f;
            m_Material.SetFloat("_movingBool", 1f);        
        }        
    }

    public async void OnTriggerEnter() {        
        player = GameObject.FindWithTag("Player");
        playerPos = player.transform.position;
        playerDir = currentPos - playerPos;
        talkRotation = Quaternion.LookRotation(-playerDir);           
        m_ForwardInput = 0.0f;
        gotClicked = true;        
        await Task.Delay(5000);
        gotClicked = false;
        lookRotation = Quaternion.LookRotation(direction);
        m_ForwardInput = 0.2f;
        turnSpeed = 1.0f;
        m_Animator.SetTrigger("Approached");          
        

    }
        

    
}
