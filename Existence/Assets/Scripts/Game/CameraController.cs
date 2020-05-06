
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : GameSystem
{
    [Header("General")]
    public Transform target;            // who are we looking at

    [Header("Offsetting")]
    public Vector3 positionOffset;      // where are we relative to target
    public Vector3 rotationOffset;      // where do we look relative to the target

    [Header("Smoothing")]
    public float movementSmoothing;     // how fast do we move to the target position
    public float rotationSmoothing;     // how fast do we look at the target
    public bool usePositionSmoothing;   // whether or not to smooth position
    public bool useRotationSmoothing;   // whether or not to smooth rotation
    public float zoomSmoothing;         // how fast to zoom in and out during scroll

    [Header("Collision")]
    public LayerMask collisionLayer;    // objects that collision respond to
    public float collisionPadding;      // how much additional offsetting? (higher values for less clipping)

    [Header("Input")]
    public float leftClickTurnSpeed;            // speed to rotate around target when left clicking
    public float minimumInspectAngle=-25.0f;    // min X angle when left click inspecting
    public float maximumInspectAngle=70.0f;     // max X angle when left click inspecting
    public float minZoom;                       // min zoom (affects positionOffset.z)
    public float maxZoom;                       // max zoom (affects positionOffset.z)

    private Camera m_Camera;
    private Vector3 m_TargetPos;
    private Vector3 m_LookPos;
    private Vector3 m_CollisionAdjustment;
    private Quaternion m_TargetRotation;
    private Quaternion m_MouseRotation;
    private float m_RotationOffsetX;
    private float m_RotationOffsetY;
    private float m_MouseX;
    private float m_MouseY;
    private bool m_HoldingLeftClick;
    private bool m_HoldingRightClick;
    private float m_Zoom;
    private bool m_UIInteraction;

#region Unity Functions
    private void Awake() {
        m_Camera = GetComponent<Camera>();
        m_MouseRotation = Quaternion.identity;
    }

    private void OnEnable() {
        UIHandle.StartedUsing += OnUIHandleInteractionStart;
        UIHandle.StoppedUsing += OnUIHandleInteractionStop;
    }

    private void OnDisable() {
        UIHandle.StartedUsing -= OnUIHandleInteractionStart;
        UIHandle.StoppedUsing -= OnUIHandleInteractionStop;
    }

    // The assumption is that all camera updates..
    // ..depend on movement from the target..
    // ..where the target is processed just before inside Update()
    private void LateUpdate() {
        if (!TargetHasIntegrity()) return;
        GetInput();
        LookAroundTarget();
        //Zoom();
        MoveToTarget();
        LookAtTarget();
        DetectCollision();	
    }
#endregion

#region Private Functions
    private void OnUIHandleInteractionStart(int _id) {
        m_UIInteraction = true;
    }

    private void OnUIHandleInteractionStop(int _id) {
        m_UIInteraction = false;
    }

    private bool TargetHasIntegrity() {
        if (target == null) {
            LogWarning("Could not find target. Please assign in the inspector..");
            return false;
        }
        return true;
    }

    private void GetInput() {
        m_HoldingLeftClick = Input.GetMouseButton(0);
        m_HoldingRightClick = Input.GetMouseButton(1);
        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");
        m_Zoom = Input.GetAxis("Mouse ScrollWheel");
    }

    private void LookAroundTarget() {
        if (m_HoldingLeftClick && !m_UIInteraction) {
            m_RotationOffsetX -= m_MouseY * leftClickTurnSpeed;
            m_RotationOffsetY += m_MouseX * leftClickTurnSpeed;
            m_RotationOffsetX = Mathf.Clamp(m_RotationOffsetX, minimumInspectAngle, maximumInspectAngle);
        } else if (m_HoldingRightClick) {
            m_RotationOffsetX -= m_MouseY * leftClickTurnSpeed;
            m_RotationOffsetX = Mathf.Clamp(m_RotationOffsetX, minimumInspectAngle, maximumInspectAngle);
        }
        m_MouseRotation = Quaternion.Euler(m_RotationOffsetX, m_RotationOffsetY, 0);
    }

    private void Zoom() {
        positionOffset.z += m_Zoom * zoomSmoothing * Time.deltaTime;
        positionOffset.z = Mathf.Clamp(positionOffset.z, minZoom, maxZoom);
    }

    private void MoveToTarget() {
        m_TargetPos = target.position + (target.rotation * m_MouseRotation) * positionOffset;
        if (usePositionSmoothing) {
            transform.position = Vector3.Lerp(transform.position, m_TargetPos, movementSmoothing * Time.deltaTime);
        } else {
            transform.position = m_TargetPos;
        }
    }

    private void LookAtTarget() {
        m_LookPos = target.position + target.right * positionOffset.x + target.up * positionOffset.y;
        m_TargetRotation = Quaternion.LookRotation(m_LookPos - transform.position, Vector3.up);
        m_TargetRotation *= Quaternion.Euler(rotationOffset);
        if (useRotationSmoothing) {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, rotationSmoothing * Time.deltaTime);
        } else {
            transform.rotation = m_TargetRotation;
        }
    }

    private void DetectCollision() {
        // get clip points
        Vector3 _upperLeft = m_Camera.ScreenToWorldPoint(new Vector3(0, Screen.height-1, m_Camera.nearClipPlane+0.0001f));
        Vector3 _upperRight = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width-1, Screen.height-1, m_Camera.nearClipPlane+0.0001f));
        Vector3 _lowerLeft = m_Camera.ScreenToWorldPoint(new Vector3(0, 0, m_Camera.nearClipPlane+0.0001f));
        Vector3 _lowerRight = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width-1, 0, m_Camera.nearClipPlane+0.0001f));
        Vector3 _center = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width/2.0f, Screen.height/2.0f, m_Camera.nearClipPlane+0.0001f));

        // draw debugging rays
        if (debug) {
            Debug.DrawLine(m_LookPos, _upperLeft, Color.green);
            Debug.DrawLine(m_LookPos, _upperRight, Color.green);
            Debug.DrawLine(m_LookPos, _lowerLeft, Color.green);
            Debug.DrawLine(m_LookPos, _lowerRight, Color.green);
            Debug.DrawLine(m_LookPos, _center, Color.green);
        }

        // check raycasts
        float _collisionOffset = 0;
        _collisionOffset = AggregateClipPointCollisionOffset(_center, _collisionOffset);
        _collisionOffset = AggregateClipPointCollisionOffset(_upperLeft, _collisionOffset);
        _collisionOffset = AggregateClipPointCollisionOffset(_upperRight, _collisionOffset);
        _collisionOffset = AggregateClipPointCollisionOffset(_lowerLeft, _collisionOffset);
        _collisionOffset = AggregateClipPointCollisionOffset(_lowerRight, _collisionOffset);
        if (_collisionOffset > 0)
            _collisionOffset += collisionPadding;

        transform.position += transform.forward * _collisionOffset;
    }

    private float AggregateClipPointCollisionOffset(Vector3 _clip, float _maxDistance) {
        RaycastHit _hit;
        if (Physics.Raycast(m_LookPos, _clip - m_LookPos, out _hit, Vector3.Distance(_clip, m_LookPos), collisionLayer)) {
            if (debug) {
                Debug.DrawLine(_hit.point, _clip, Color.red);
            }
            float _distance = Vector3.Distance(_hit.point, _clip);
            if (_distance > _maxDistance) {
                _maxDistance = _distance;
            }
        }
        return _maxDistance;
    }
#endregion
}
