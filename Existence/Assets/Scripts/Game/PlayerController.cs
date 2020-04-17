
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    public float runSpeed = 6.0f;
    public float runStrafeSpeed = 3.0f;
    public float walkStrafeSpeed = 3.0f;
    public float walkSpeed = 1.0f;
    public float backwardSpeed = 0.45f;
    public float turnSpeed = 20.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private CharacterController m_Controller;
    private Animator m_Animator;
    private Vector3 m_MoveDirection = Vector3.zero;
    private float m_ForwardInput;
    private float m_TurnInput;
    private float m_StrafeInput;
    private float m_StrafeAnimation;
    private bool m_JumpInput;
    private bool m_RightMouseDown;
    private bool m_ShiftIsDown;
    private float m_VerticalAxis;
    private float m_VerticalAxisRaw;
    private float m_HorizontalAxis;
    private float m_HorizontalAxisRaw;

    public float runAnimation { get {return m_ForwardInput;} }
    public float strafeAnimation { get {return m_StrafeAnimation;} }
    public bool grounded { 
        get {
            return m_Controller && m_Controller.isGrounded;
        }
    }

#region Unity Functions
    private void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Animate();
    }
#endregion

#region Private Functions
    private void GetInput() {
        m_VerticalAxisRaw = Input.GetAxisRaw("Vertical");
        m_VerticalAxis = Input.GetAxis("Vertical");
        m_HorizontalAxis = Input.GetAxis("Horizontal");
        m_HorizontalAxisRaw = Input.GetAxisRaw("Horizontal");
        m_RightMouseDown = Input.GetMouseButton(1);
        m_ShiftIsDown = Input.GetKey(KeyCode.LeftShift);
        m_JumpInput = Input.GetButton("Jump");
        m_TurnInput = m_RightMouseDown ? Input.GetAxis("Mouse X")*3 : m_HorizontalAxis;
        m_StrafeInput = m_RightMouseDown && Mathf.Abs(m_HorizontalAxis) > 0 ? 
                        Mathf.Lerp(m_StrafeInput, m_HorizontalAxis, 4 * Time.deltaTime) : ApproachZero(m_StrafeInput);
        m_StrafeAnimation = Mathf.Abs(m_VerticalAxisRaw) == 0 ? Mathf.Lerp(m_StrafeAnimation, m_StrafeInput, 5*Time.deltaTime) : ApproachZero(m_StrafeAnimation);
        DetectWalking();
    }

    private void Move() {
        if (grounded)
        {
            // calculate forward speed
            float _forwardSpeed = m_ForwardInput < 0 ? backwardSpeed : m_ShiftIsDown ? walkSpeed : runSpeed;
            float _strafeSpeed = m_ShiftIsDown || m_VerticalAxisRaw < 0 ? walkStrafeSpeed : runStrafeSpeed;
            float _netSpeed = Mathf.Abs(m_ForwardInput) > 0 && Mathf.Abs(m_HorizontalAxis) > 0 ? (_forwardSpeed + _strafeSpeed) / 2.0f : 
                              Mathf.Abs(m_ForwardInput) > 0 ? _forwardSpeed : Mathf.Abs(m_HorizontalAxis) > 0 ? _strafeSpeed : 0.0f;
            m_MoveDirection = (transform.forward * m_ForwardInput + transform.right * m_StrafeInput) * _netSpeed;

            if (m_JumpInput)
                m_MoveDirection.y = jumpSpeed;
        }

        m_MoveDirection.y -= gravity * Time.deltaTime;
        m_Controller.Move(m_MoveDirection * Time.deltaTime);
    }

    private void DetectWalking() {
        if (m_ShiftIsDown) {
            if (m_VerticalAxis > 0.5f) {
                m_ForwardInput = Mathf.Lerp(m_ForwardInput, 0.5f, 2 * Time.deltaTime);
            } else {
                m_ForwardInput = m_VerticalAxis;
            }
        } else {
            m_ForwardInput = Mathf.Lerp(m_ForwardInput, m_VerticalAxis, 8 * Time.deltaTime);
        }
    }

    private void HandleRunning() {
        // handle running/walking forward and backward
        // should run the same way using diagonal input
        float _speed = m_VerticalAxis < 0 ? backwardSpeed : m_VerticalAxis < 0.5f ? walkSpeed : runSpeed;
        m_MoveDirection = transform.forward * m_ForwardInput * _speed;
    }

    private void Turn() {
        transform.rotation *= Quaternion.Euler(0.0f, m_TurnInput * turnSpeed * Time.deltaTime, 0.0f);
    }

    private float ApproachZero(float _val) {
        _val = Mathf.Lerp(_val, 0, 5 * Time.deltaTime);
        if (Mathf.Abs(_val) <= 0.025f) {
            _val = 0;
        }
        return _val;
    }

    private void Animate() {
        m_Animator.SetFloat("running", m_ForwardInput);
        m_Animator.SetFloat("strafing", m_StrafeAnimation);
        m_Animator.SetBool("grounded", grounded);
    }
#endregion
}
