using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : GameSystem
{
    public static SelectionController instance;

    private RaycastHit m_HitInfo;
    private Selectable m_Selection;

    public Selectable selection {
        get {
            return m_Selection;
        } set {
            m_Selection = value;
        }
    }

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HitCheck();
        }   
    }
#endregion

#region Private Functions
    private void HitCheck() {
        if (m_Selection) {
            m_Selection.nameplateData.isVisible = false;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_HitInfo)) {
            Selectable _s = m_HitInfo.collider.gameObject.GetComponent<Selectable>();
            if (!_s) return;
            
            _s.nameplateData.isVisible = true;
            m_Selection = _s;
        }
    }
#endregion
}
