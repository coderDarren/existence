
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerController : GameSystem
{

    [Header("Running")]
    public float runSpeed = 6.0f;
    public float walkSpeed = 1.0f;
    public float backwardSpeed = 0.45f;

    [Header("Strafing")]
    public float walkStrafeSpeed = 3.0f;
    public float runStrafeSpeed = 3.0f;

    [Header("Turning")]
    public float turnSpeed = 20.0f;

    [Header("Jumping")]
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Grounding")]
    [Range(4,12)]
    public int groundCheckDensity = 5;
    public float groundCheckRadius = 1.0f;
    public float groundCheckDist = 1.0f;
    public float groundCheckBias = 0.1f;
    public LayerMask groundLayer;

    private Player m_Player;
    private CharacterController m_Controller;
    private Animator m_Animator;
    private Vector3 m_MoveDirection;
    private Vector3 m_ForwardVec;
    private Vector3 m_RightVec;
    private Quaternion m_DeltaRotation;
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
    private bool m_Grounded;
    private float m_GroundBiasTimer;
    private bool m_InputIsFrozen;
    private float m_AnimationSpeed;

    public float runAnimation { get {return m_ForwardInput;} }
    public float strafeAnimation { get {return m_StrafeAnimation;} }
    public bool grounded { get { return m_Grounded; } }

#region Unity Functions
    private void Start()
    {
        m_Player = GetComponent<Player>();
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

#region Public Functions
    public void FreezeInput() {
        m_InputIsFrozen = true;
    }

    public void FreeInput() {
        m_InputIsFrozen = false;
    }
#endregion

#region Private Functions
    private void DecayInput() {
        m_VerticalAxis = Mathf.Lerp(m_VerticalAxis, 0, 5*Time.deltaTime);
        m_VerticalAxisRaw = Mathf.Lerp(m_VerticalAxisRaw, 0, 5*Time.deltaTime);
        m_HorizontalAxis = Mathf.Lerp(m_HorizontalAxis, 0, 5*Time.deltaTime);
        m_HorizontalAxisRaw = Mathf.Lerp(m_HorizontalAxisRaw, 0, 5*Time.deltaTime);
        m_TurnInput = Mathf.Lerp(m_TurnInput, 0, 5*Time.deltaTime);
        m_StrafeInput = Mathf.Lerp(m_StrafeInput, 0, 5*Time.deltaTime);
        m_ForwardInput = Mathf.Lerp(m_ForwardInput, 0, 5*Time.deltaTime);
        m_RightMouseDown = false;
        m_ShiftIsDown = false;
        m_JumpInput = false;
    }

    private void GetInput() {
        if (m_InputIsFrozen) {
            DecayInput();
            return;
        }
        m_VerticalAxisRaw = Input.GetAxisRaw("Vertical");
        m_VerticalAxis = Input.GetAxis("Vertical");
        m_HorizontalAxis = Input.GetAxis("Horizontal");
        m_HorizontalAxisRaw = Input.GetAxisRaw("Horizontal");
        m_RightMouseDown = Input.GetMouseButton(1);
        m_ShiftIsDown = Input.GetKey(KeyCode.LeftShift);
        m_JumpInput = Input.GetButton("Jump");
        m_TurnInput = m_RightMouseDown ? Input.GetAxis("Mouse X")*3 : m_HorizontalAxis;
        m_StrafeInput = m_RightMouseDown && Mathf.Abs(m_HorizontalAxis) > 0 ? m_HorizontalAxis : 0;
        m_StrafeAnimation = Mathf.Abs(m_VerticalAxisRaw) == 0 ? Mathf.Lerp(m_StrafeAnimation, m_StrafeInput, 5*Time.deltaTime) : ApproachZero(m_StrafeAnimation);
        DetectWalking();
    }

    private void Move() {
        CheckGrounded();
        if (m_Controller.isGrounded)
        {
            // calculate forward speed
            var _playerStats = m_Player.GetAggregatedStats();
            m_AnimationSpeed = m_ForwardInput <= 0 || m_ShiftIsDown ? 1 : (0.75f + 0.00045f*_playerStats.runSpeed);
            m_AnimationSpeed = Mathf.Clamp(m_AnimationSpeed, 0.25f, 3);
            float _forwardSpeed = m_ForwardInput < 0 ? backwardSpeed : m_ShiftIsDown ? walkSpeed : (runSpeed+0.01f*_playerStats.runSpeed);
            _forwardSpeed = Mathf.Clamp(_forwardSpeed, 0.25f, Mathf.Infinity);
            float _strafeSpeed = m_ShiftIsDown || m_VerticalAxisRaw < 0 ? walkStrafeSpeed : runStrafeSpeed;
            m_MoveDirection = m_ForwardVec * m_ForwardInput * _forwardSpeed + m_RightVec * m_StrafeInput * _strafeSpeed;

            if (m_JumpInput)
                m_MoveDirection.y = jumpSpeed + 0.01f*_playerStats.strength;
        } 
        m_MoveDirection.y -= gravity * Time.deltaTime;
        m_Controller.Move(m_MoveDirection * Time.deltaTime);
    }

    private void DetectWalking() {
        if (m_ShiftIsDown) {
            if (m_VerticalAxis > 0.5f) {
                m_ForwardInput = 0.5f;
            } else {
                m_ForwardInput = m_VerticalAxis;
            }
        } else {
            m_ForwardInput = m_VerticalAxis;
        }
    }

    private void Turn() {
        m_DeltaRotation = Quaternion.Euler(0.0f, m_TurnInput * turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation *= m_DeltaRotation;
    }

    private void CheckGrounded() {
        RaycastHit _hitInfo;
        Vector3 _from = transform.position + m_Controller.center;
        Vector3 _normalAggregate = Vector3.zero;
        int _hits = 0;
        bool _hit = false;

        // check center
        if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
            _normalAggregate += _hitInfo.normal;
            _hit = true;
            _hits++;
            if (debug) {
                Debug.DrawLine(_from, _from + Vector3.down * groundCheckDist, Color.green);
            }
        }

        // check radius
        for (int i = 0; i < groundCheckDensity; i++) {
            float _angle = (360.0F / groundCheckDensity) * i;
            _from = transform.position + m_Controller.center + (Quaternion.Euler(0, _angle, 0) * Vector3.forward) * groundCheckRadius;
            if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
                _normalAggregate += _hitInfo.normal;
                _hit = true;
                _hits++;
            }
            if (debug) {
                Debug.DrawLine(_from, _from + Vector3.down * groundCheckDist, Color.green);
            }
        }

        // handle hit
        // calculate forward and right vectors
        if (_hit) {
            m_Grounded = true;
            m_GroundBiasTimer = 0;
            Vector3 _avgNormal = _normalAggregate / _hits;
            m_ForwardVec = Vector3.Cross(transform.right, _avgNormal);
            m_RightVec = Vector3.Cross(_avgNormal, m_ForwardVec);
        } else {
            // Don't "unground" until bias is reached
            if (m_Grounded) {
                m_GroundBiasTimer += Time.deltaTime;
                if (m_GroundBiasTimer > groundCheckBias) {
                    m_Grounded = false;
                }
            }
        }
        
        if (debug) {
            Debug.DrawLine(transform.position, transform.position + m_ForwardVec * 1.0f, Color.blue);
            Debug.DrawLine(transform.position, transform.position + m_RightVec * 1.0f, Color.red);
        }
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
        m_Animator.speed = m_AnimationSpeed;
    }
#endregion
}
