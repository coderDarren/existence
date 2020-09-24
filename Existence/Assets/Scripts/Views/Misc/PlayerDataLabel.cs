using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PlayerDataLabel : GameSystem
{
    public enum DataType {
        TIX
    }

    public DataType dataType;
    public string prefix;
    public string suffix;
    public bool lerpInt;
    public float lerpTime;

    private Session m_Session;
    private Text m_Label;

    // Paramters to assist with int data lerps
    private int m_LastInt;
    private int m_StartInt;
    private int m_CurrentInt;

    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogWarning("PlayerDataLabel: "+gameObject.name+" is trying to access session, but no instance was found.");
            }
            return m_Session;
        }
    }

#region Unity Functions
    private void Awake() {
        // Guaranteed to be not-null because of this class's attribute
        m_Label = GetComponent<Text>();
    }

    private void Start() {
        m_Label.text = DataString();
    }

    private void OnDisable() {
        StopCoroutine("LerpInt");
    }

    private void Update() {
        if (!session) return;

        if (lerpInt) {
            if (IntChanged()) {
                StartIntLerp();
            }
        } else {
            m_Label.text = DataString();
        }
    }
#endregion

#region Private Functions
    private bool IntChanged() {
        bool _ret = false;

        switch (dataType) {
            case DataType.TIX:  m_CurrentInt = session.player.data.player.tix; break;
            default: break;
        }

        if (m_CurrentInt != m_LastInt) {
            _ret = true;
            m_StartInt = m_LastInt;
        }

        m_LastInt = m_CurrentInt;

        return _ret;
    }

    private void StartIntLerp() {
        StopCoroutine("LerpInt");
        StartCoroutine("LerpInt");
    }

    private string DataString() {
        string _ret = string.Empty;
        switch (dataType) {
            case DataType.TIX: _ret = session.player.data.player.tix.ToString(); break;
            default: break;
        }
        return prefix + _ret + suffix;
    }

    private IEnumerator LerpInt() {
        float _timer = 0;
        string _label = string.Empty;

        while (_timer < lerpTime) {
            _label = prefix+((int)Mathf.Lerp(m_StartInt, m_CurrentInt, _timer / lerpTime)).ToString()+suffix;
            m_Label.text = _label;
            _timer += Time.deltaTime;
            yield return null;
        }

        m_Label.text = DataString();
    }
#endregion
}
