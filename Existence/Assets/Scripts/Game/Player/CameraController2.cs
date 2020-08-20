
using UnityEngine;

public class CameraController2 : GameSystem
{
    [Header("General")]
    public Transform target;

    [Header("Controls")]
    public float followSpeed;
    public float lookSpeed;
    public float turnSpeed;
    public float autoFocusTime;
    public float idleTurnAngleLimit;
    public float strafeAngle;

    [Header("Offsetting")]
    public Vector3 targetOffset;        // offset of target's true position
    public Vector3 followOffset;      // where are we relative to target
    public Vector3 rotationOffset;      // where do we look relative to the target

    [Header("Input")]
    public float minimumInspectAngle=-25.0f;    // min X angle when left click inspecting
    public float maximumInspectAngle=70.0f;     // max X angle when left click inspecting
    public float mouseClickTurnSpeed;
    public float verticalMouseTurnSpeed;

    // Components
    private PlayerController2 m_Player;

    // Inputs
    private bool m_RightClick;
    private bool m_LeftClick;
    private bool m_VerticalStart;
    private bool m_HorizontalStart;
    private float m_HorizontalRaw;
    private float m_Horizontal;
    private float m_VerticalRaw;
    private float m_Vertical;
    private float m_MouseX;
    private float m_MouseY;

    // Interaction
    private bool m_UIInteraction;
    // the params below used for TurnTargetTowardCameraDirection()
    private bool m_AutoFocus;   // control param to determine if TurnTargetTowardCameraDirection() should ever execute
    private bool m_AutoFocusStarted;
    private float m_CameraAngleStart;
    private Quaternion m_PlayerAngleStart;
    private Quaternion m_PlayerAngleGoal;
    private float m_Timer;

    // Vectors
    private Vector3 m_TargetPos;
    private Vector3 m_LookPos;
    private Vector2 m_MouseRotationOffset;
    private Quaternion m_TargetRotation;
    private Quaternion m_MouseRotation;

    // Control
    private float m_StrafeLockAngle;
    private float m_StrafeAngle;

    private PlayerController2 player {
        get {
            if (!target) {
                LogWarning("No target found.");
                return null;
            }
            if (!m_Player) {
                m_Player = target.GetComponent<PlayerController2>();
            }
            if (!m_Player) {
                LogWarning("Target must have PlayerController attached");
            }
            return m_Player;
        }
    }

#region Unity Functions
    private void LateUpdate() {
        PollInput();

        if (m_RightClick && m_Horizontal != 0) {
            StrafeDrivePlayer();
        } else if (m_VerticalRaw != 0) {
            DrivePlayer();
        } else if ((m_RightClick || m_HorizontalRaw != 0) && !m_LeftClick && m_VerticalRaw == 0) {
            TurnTargetWithCameraDirection();
        } else if (m_LeftClick && !m_RightClick && !m_UIInteraction) {
            RotateAroundTargetWithClick();
        } else if (!m_LeftClick && !m_RightClick && m_AutoFocus && m_HorizontalRaw == 0) {
            TurnTargetTowardCameraDirection();
        }

        if (m_RightClick) {
           m_MouseRotationOffset.x -= m_MouseY * verticalMouseTurnSpeed * Time.deltaTime;
           m_MouseRotationOffset.x = Mathf.Clamp(m_MouseRotationOffset.x, minimumInspectAngle, maximumInspectAngle);
        }

        ApplyCameraPosition();
        ApplyCameraRotation();
    }

    private void OnEnable() {
        m_AutoFocus = true;
        UIHandle.StartedUsing += OnUIHandleInteractionStart;
        UIHandle.StoppedUsing += OnUIHandleInteractionStop;
    }

    private void OnDisable() {
        UIHandle.StartedUsing -= OnUIHandleInteractionStart;
        UIHandle.StoppedUsing -= OnUIHandleInteractionStop;
    }
#endregion

#region Public Functions

#endregion

#region Private Functions
    private void OnUIHandleInteractionStart(int _id) {
        m_UIInteraction = true;
    }

    private void OnUIHandleInteractionStop(int _id) {
        m_UIInteraction = false;
    }
    
    private void PollInput() {
        m_RightClick = Input.GetMouseButton(1);
        m_LeftClick = Input.GetMouseButton(0);
        m_HorizontalRaw = Input.GetAxisRaw("Horizontal");
        m_Horizontal = Input.GetAxis("Horizontal");
        m_VerticalRaw = Input.GetAxisRaw("Vertical");
        m_Vertical = Input.GetAxis("Vertical");
        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonUp(1)) {
            m_AutoFocus = true;
        } else if (Input.GetMouseButtonUp(0)) {
            m_AutoFocus = false;
        }

        if (m_HorizontalRaw != 0 && !m_HorizontalStart) {
            m_AutoFocusStarted = false;
            m_HorizontalStart = true;
            m_AutoFocus = true;
            m_Timer = 0;
        } else if (m_HorizontalRaw == 0) {
            m_HorizontalStart = false;
        }
        
        if (m_VerticalRaw != 0 && !m_VerticalStart) {
            m_AutoFocusStarted = false;
            m_VerticalStart = true;
            m_AutoFocus = true;
            m_Timer = 0;
        } else if (m_VerticalRaw == 0) {
            m_VerticalStart = false;
        }
    }

    /*
     * Rotate the player toward the direction of the camera while the player is strafing
     */
    private void StrafeDrivePlayer() {
        Log("StrafeDrivePlayer");
        
        m_StrafeLockAngle += m_MouseX * mouseClickTurnSpeed * Time.deltaTime;
        float _target = m_HorizontalRaw != 0 && m_VerticalRaw > 0 ? strafeAngle * m_HorizontalRaw :
                        m_HorizontalRaw != 0 && m_VerticalRaw < 0 ? -strafeAngle * m_HorizontalRaw :
                        m_HorizontalRaw > 0 && m_VerticalRaw == 0 ? 90 :
                        m_HorizontalRaw < 0 && m_VerticalRaw == 0 ? -90 : 0;

        if (m_VerticalRaw != 0) {
            m_StrafeAngle = Mathf.Lerp(m_StrafeAngle, _target, 5 * Time.deltaTime);
        } else {
            m_StrafeAngle = _target;
        }
        
        player.SetRotationImmediate(Quaternion.Euler(0, m_StrafeLockAngle + m_StrafeAngle, 0));
        m_MouseRotationOffset.y = -m_StrafeAngle;
    }

    /*
     * Rotate the player toward the direction of the camera while the player is running
     */
    private void DrivePlayer() {
        Log("DrivePlayer");

        m_StrafeLockAngle = transform.eulerAngles.y;
        m_StrafeAngle = 0;

        if (m_LeftClick) {
            RotateAroundTargetWithClick();
        } else if (m_MouseRotationOffset.y != 0) {
            m_MouseRotationOffset.y = 0;
            player.SetRotationImmediate(Quaternion.Euler(0, m_StrafeLockAngle, 0));
        }

        float _turnSpeed = m_HorizontalRaw != 0 ? m_HorizontalRaw * turnSpeed * Time.deltaTime :
                            m_RightClick ? m_MouseX * mouseClickTurnSpeed * Time.deltaTime : 0;
        player.Rotate(_turnSpeed);
    }

    /*
     * Rotate camera around player without rotating the target, and..
     * ..while the target is not moving
     */
    private void RotateAroundTargetWithClick() {
        Log("RotateAroundTargetWithClick");
        m_StrafeLockAngle = transform.eulerAngles.y;

        m_MouseRotationOffset.x -= m_MouseY * mouseClickTurnSpeed * Time.deltaTime;
        m_MouseRotationOffset.y += m_MouseX * mouseClickTurnSpeed * Time.deltaTime;
        m_MouseRotationOffset.x = Mathf.Clamp(m_MouseRotationOffset.x, minimumInspectAngle, maximumInspectAngle);
    }

    /*
     * While camera is being turned, turn the target while target is not moving
     */
    private void TurnTargetWithCameraDirection() {
        Log("TurnTargetWithCameraDirection"); 
        m_AutoFocusStarted = false;
        m_StrafeLockAngle = transform.eulerAngles.y;

        float _turnSpeed = m_HorizontalRaw != 0 ? m_HorizontalRaw * turnSpeed * Time.deltaTime :
                            m_RightClick ? m_MouseX * mouseClickTurnSpeed * Time.deltaTime : 0;

        m_MouseRotationOffset.y += _turnSpeed;
        m_MouseRotationOffset.y = Utilities.ClampAngle(m_MouseRotationOffset.y, -180, 180);

        if (m_MouseRotationOffset.y <= -idleTurnAngleLimit) {
            m_MouseRotationOffset.y = Utilities.ClampAngle(Mathf.Lerp(m_MouseRotationOffset.y, -idleTurnAngleLimit, 5 * Time.deltaTime), -180, 180);
        } else if (m_MouseRotationOffset.y >= idleTurnAngleLimit) {
            m_MouseRotationOffset.y = Utilities.ClampAngle(Mathf.Lerp(m_MouseRotationOffset.y, idleTurnAngleLimit, 5 * Time.deltaTime), -180, 180);
        }

        if (Mathf.Abs(m_MouseRotationOffset.y) >= idleTurnAngleLimit) {
            player.Rotate(_turnSpeed);
        }
    }

    /*
     * While camera is static, turn the target to the direction of the camera
     */
    private void TurnTargetTowardCameraDirection() {
        Log("TurnTargetTowardCameraDirection");
        m_StrafeLockAngle = transform.eulerAngles.y;

        if (!m_AutoFocusStarted) {
            m_AutoFocusStarted = true;
            m_CameraAngleStart = transform.eulerAngles.y;
            m_PlayerAngleStart = Quaternion.Euler(0, target.eulerAngles.y, 0);
            m_PlayerAngleGoal = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            m_Timer = 0;
        } else {
            m_Timer += Time.deltaTime;
            player.SetRotation(Quaternion.Lerp(m_PlayerAngleStart, m_PlayerAngleGoal, m_Timer / autoFocusTime));
            m_MouseRotationOffset.y = m_CameraAngleStart - target.eulerAngles.y;
        }
    }

    /*
     * Look at the target
     */ 
    private void ApplyCameraRotation() {
        m_LookPos = target.position + targetOffset + target.right * followOffset.x + target.up * followOffset.y;
        m_TargetRotation = Quaternion.LookRotation(m_LookPos - transform.position, Vector3.up);
        m_TargetRotation *= Quaternion.Euler(rotationOffset);
        transform.rotation = m_TargetRotation;
    }

    /*
     * Find position in the world relative to the target
     */
    private void ApplyCameraPosition() {
        m_MouseRotation = Quaternion.Euler(m_MouseRotationOffset.x, m_MouseRotationOffset.y, 0);
        m_TargetPos = target.position + targetOffset + (target.rotation * m_MouseRotation) * followOffset;
        transform.position = m_TargetPos;
    }
#endregion
}
