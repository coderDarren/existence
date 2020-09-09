﻿using UniRx.Async;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
public class PlayerController : GameSystem
{

    [Header("Forces")]
    public float runSpeed = 6.0F;
    public float walkSpeed = 2.0F;
    public float jumpForce = 4.0F;
    public float gravity = 0.15F;

    [Header("Grounding")]
    [Range(4,24)]
    public int groundCheckDensity = 12;
    public float groundCheckRadius = 0.3F;
    public float groundCheckDist = 1.2F;
    public float groundCheckBias = 0.1F;
    public LayerMask groundLayer;

    // Components
    private Player m_Player;
    private NetworkPlayer m_NetworkPlayer;
    private CharacterController m_Character;
    private Vector3 m_MoveDirection;
    private Quaternion m_TargetRotation;
    private Animator m_Animator;

    // Inputs
    private bool m_Update;
    private bool m_InputIsFrozen;
    private bool m_RightClick;
    private bool m_Jump;
    private bool m_Shift;
    private float m_HorizontalRaw;
    private float m_Horizontal;
    private float m_VerticalRaw;
    private float m_Vertical;

    // running is a cumulative representation of "forward" which includes strafing and running
    // value is based on m_RunningAnim
    // These values are a raw representation which means values belong to the set [-1,0,1]
    // Why? Because we use this raw value to determine when to send input changes to the network
    // ..we don't want to send continuous changes from [-1,1]
    private float m_RunningRaw;
    private float m_LastRunningRaw;

    // Physics
    private bool m_Grounded;
    private bool m_LastGrounded;
    private float m_GroundBiasTimer;
    private float m_Gravity;

    // Animation
    private float m_RunAnimationSpeed;
    private float m_RunningAnim;

    // Player Data
    private StatData m_PlayerStats;

    private CharacterController character {
        get {
            if (!m_Character) {
                m_Character = GetComponent<CharacterController>();
            }
            return m_Character;
        }
    }

    public float runInput { get {return m_Vertical;} }
    public bool grounded { get { return m_Grounded; } }

#region Unity Functions
    private void Start() {
        m_NetworkPlayer = GetComponent<NetworkPlayer>();
        m_Player = GetComponent<Player>();
        m_Animator = GetComponent<Animator>();
        m_TargetRotation = transform.rotation;
        m_Update = true;
        m_InputIsFrozen = false;
    }

    private void Update() {
        if (!m_Update) return;
        m_PlayerStats = m_Player.GetAggregatedStats();

        PollInput();
        PollInputChange();
        CheckGrounded();
        Jump();
        Move();
        Turn();
        Animate();
    }
#endregion

#region Public Functions
    public async void PauseUpdates(int _time) {
        m_Update = false;
        await UniTask.Delay(_time);
        m_Update = true;
    }

    public void FreezeInput() {
        m_InputIsFrozen = true;
    }

    public void FreeInput() {
        m_InputIsFrozen = false;
    }

    public void Rotate(float _angle) {
        m_TargetRotation *= Quaternion.Euler(0, _angle, 0);
    }

    public void SetRotation(Quaternion _rot) {
        m_TargetRotation = _rot;
    }

    public void SetRotationImmediate(Quaternion _rot) {
        transform.rotation = _rot;
        m_TargetRotation = transform.rotation;
    }
#endregion

#region Private Functions
    private void DecayInput() {
        m_Vertical = Mathf.Lerp(m_Vertical, 0, 5*Time.deltaTime);
        m_VerticalRaw = Mathf.Lerp(m_VerticalRaw, 0, 5*Time.deltaTime);
        m_Horizontal = Mathf.Lerp(m_Horizontal, 0, 5*Time.deltaTime);
        m_HorizontalRaw = Mathf.Lerp(m_HorizontalRaw, 0, 5*Time.deltaTime);
        m_Shift = false;
    }

    private void PollInput() {
        if (m_InputIsFrozen) {
            DecayInput();
            return;
        }

        m_RightClick = Input.GetMouseButton(1);
        m_Jump = Input.GetButton("Jump");
        m_Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        m_HorizontalRaw = Input.GetAxisRaw("Horizontal");
        m_Horizontal = Input.GetAxis("Horizontal");
        m_VerticalRaw = Input.GetAxisRaw("Vertical");
        m_Vertical = Input.GetAxis("Vertical");
    }

    /* 
     * Input changes used to determine what to send up the network
     */
    private void PollInputChange() {
        if (m_LastGrounded != m_Grounded) {
            m_NetworkPlayer.Network_WriteAnimBool("grounded", m_Grounded);
        }

        if (m_LastRunningRaw != m_RunningRaw) {
            m_NetworkPlayer.Network_WriteAnimFloat("running", m_RunningRaw);
        }

        m_LastGrounded = m_Grounded;
        m_LastRunningRaw = m_RunningRaw;
    }

    private void Jump() {
        if (m_Grounded && m_Jump) {
            m_MoveDirection.y = jumpForce + 0.01f * (0.01f+m_PlayerStats.strength) * Time.deltaTime;
        } else if (!m_Grounded) {
            m_MoveDirection.y -= gravity * Time.deltaTime;
        }
    }

    private void Move() {
        Vector3 _dir = m_RightClick && m_VerticalRaw == 0 && m_HorizontalRaw != 0 ? transform.forward : transform.forward * m_VerticalRaw;
        float _speed = m_VerticalRaw < 0 || m_Shift ? walkSpeed : (runSpeed + 0.01f * m_PlayerStats.runSpeed);
        m_MoveDirection = _dir * _speed + Vector3.up * m_MoveDirection.y;
        character.Move(m_MoveDirection * Time.deltaTime);
    }

    private void Turn() {
        transform.rotation = Quaternion.Lerp(transform.rotation, m_TargetRotation, 25 * Time.deltaTime);
    }

    private void Animate() {
        m_RunAnimationSpeed = m_Vertical <= 0 || m_Shift ? 1 : (0.75f + 0.00045f * m_PlayerStats.runSpeed);
        m_RunAnimationSpeed = Mathf.Clamp(m_RunAnimationSpeed, 0.25f, 3);
        
        if (m_RightClick) { // a few different strafing cases to consider to ensure smooth animation.. 
            if (m_VerticalRaw > 0) {
                m_RunningAnim = Mathf.Abs(m_Horizontal) + Mathf.Abs(m_Vertical);
                m_RunningRaw = 1;
            } else if (m_VerticalRaw == 0) {
                m_RunningAnim = Mathf.Abs(m_Horizontal);
                m_RunningRaw = Mathf.Abs(m_HorizontalRaw);
            } else if (m_VerticalRaw < 0) {
                m_RunningAnim = m_Vertical;
                m_RunningRaw = m_VerticalRaw;
            }
        } else {
            m_RunningAnim = m_Vertical;
            m_RunningRaw = m_VerticalRaw;
        }

        m_Animator.SetFloat("running", m_RunningAnim);
        m_Animator.SetBool("grounded", m_Grounded);
        m_Animator.speed = m_RunAnimationSpeed;
    }

    private void CheckGrounded() {
        RaycastHit _hitInfo;
        Vector3 _from = transform.position + character.center;
        int _hits = 0;
        bool _hit = false;

        // check center
        if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
            _hit = true;
            _hits++;
            if (debug) {
                Debug.DrawLine(_from, _from + Vector3.down * groundCheckDist, Color.green);
            }
        }

        // check radius
        // !! TODO
        // Make this a function
        for (int i = 0; i < groundCheckDensity; i++) {
            float _angle = (360.0F / groundCheckDensity) * i;
            _from = transform.position + character.center + (Quaternion.Euler(0, _angle, 0) * Vector3.forward) * groundCheckRadius;
            if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
                _hit = true;
                _hits++;
            }
            if (debug) {
                Debug.DrawLine(_from, _from + Vector3.down * groundCheckDist, Color.green);
            }
        }

        for (int i = 0; i < groundCheckDensity; i++) {
            float _angle = (360.0F / groundCheckDensity) * i;
            _from = transform.position + character.center + (Quaternion.Euler(0, _angle, 0) * Vector3.forward) * (groundCheckRadius/2.0f);
            if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
                _hit = true;
                _hits++;
            }
            if (debug) {
                Debug.DrawLine(_from, _from + Vector3.down * groundCheckDist, Color.green);
            }
        }

        for (int i = 0; i < groundCheckDensity; i++) {
            float _angle = (360.0F / groundCheckDensity) * i;
            _from = transform.position + character.center + (Quaternion.Euler(0, _angle, 0) * Vector3.forward) * (groundCheckRadius/5.0f);
            if (Physics.Raycast(_from, Vector3.down, out _hitInfo, groundCheckDist, groundLayer)) {
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
            Debug.DrawLine(transform.position, transform.position + transform.forward * 1.0f, Color.blue);
            Debug.DrawLine(transform.position, transform.position + transform.right * 1.0f, Color.red);
        }
    }
#endregion
}
