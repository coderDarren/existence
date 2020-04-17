
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text nameLabel;
    public float moveSmooth=3;
    public bool isClient=false;
    public Session session;

    private Vector3 m_TargetPos;
    private Vector3 m_Velocity;

#region Unity Functions
    private void Start() {
        if (isClient) {
            // initialize the player
            nameLabel.text = session.playerName;
        }
    }

    private void Update() {
        if (isClient) return;
        transform.position = Vector3.SmoothDamp(transform.position, m_TargetPos, ref m_Velocity, moveSmooth);
    }
#endregion

#region Public Functions
    public void Init(string _name, Vector3 _pos) {
        if (isClient) return;
        nameLabel.text = _name;
        m_TargetPos = _pos;
        transform.position = m_TargetPos;
    }

    public void UpdatePosition(Vector3 _pos) {
        //if (isClient) return;
        m_TargetPos = _pos;
    }
#endregion
}
