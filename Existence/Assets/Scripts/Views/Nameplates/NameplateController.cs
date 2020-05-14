﻿using System.Collections;
using UnityEngine;

/// <summary>
/// The NameplateController is responsible for managing all..
/// ..viewable nameplates visible to the camera
/// </summary>
public class NameplateController : GameSystem
{
    public static NameplateController instance;

    public RectTransform container;
    public GameObject nameplate;
    public float maxViewableDistance;
    public float fadeDistance;
    public float scaleDistance;
    public float minScale=0.5f;
    public float maxScale=1;

    private Hashtable m_Nameplates;
    private Hashtable m_Selectables;

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
            m_Nameplates = new Hashtable();
            m_Selectables = new Hashtable();
        }
    }

    private void Update() {
        foreach(DictionaryEntry _entry in m_Selectables) {
            Selectable _s = (Selectable)_entry.Value;
            Nameplate _n = (Nameplate)m_Nameplates[_entry.Key];
            _n.transform.position = Camera.main.WorldToScreenPoint(_s.transform.position + _s.nameplateOffset);
            float _dist = Vector3.Distance(Camera.main.transform.position, _s.transform.position);
            _n.SetAlpha(1 - (_dist - maxViewableDistance) / fadeDistance);
            _n.SetScale(minScale + (1 - _dist / scaleDistance) * (maxScale - minScale));
            _n.SetName(_s.nameplate.name);
        }
    }
#endregion

#region Public Functions
    public void TrackSelectable(Selectable _selectable) {
        if (m_Selectables.ContainsKey(_selectable.name)) return; // already tracking
        m_Selectables.Add(_selectable.name, _selectable);
        GameObject _go = Instantiate(nameplate);
        Nameplate _nameplate = _go.GetComponent<Nameplate>();
        _nameplate.SetAlpha(0);
        _go.transform.SetParent(container);
        _go.transform.localScale = Vector3.one;
        m_Nameplates.Add(_selectable.name, _nameplate);
    }

    public void ForgetSelectable(Selectable _selectable) {
        if (!m_Selectables.ContainsKey(_selectable.name)) return; // not tracking
        m_Selectables.Remove(_selectable.name);
        Nameplate _n = (Nameplate)m_Nameplates[_selectable.name];
        m_Nameplates.Remove(_selectable.name);
        Destroy(_n.gameObject);
    }
#endregion
}
