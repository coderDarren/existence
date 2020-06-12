using System.Collections;
using UnityEngine;

/// <summary>
/// The NameplateController is responsible for managing all..
/// ..viewable nameplates visible to the camera
/// </summary>
public class NameplateController : GameSystem
{
    public static NameplateController instance;

    public float maxViewableDistance;
    public float fadeDistance;
    public float scaleDistance;
    public float minScale=0.5f;
    public float maxScale=1;

    private Hashtable m_Selectables;

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
            m_Selectables = new Hashtable();
        }
    }

    private void Update() {
        foreach(DictionaryEntry _entry in m_Selectables) {
            Selectable _s = (Selectable)_entry.Value;
            Nameplate _n = _s.nameplate;

            float _dist = Vector3.Distance(Camera.main.transform.position, _s.transform.position);
            _n.SetAlpha(1 - (_dist - maxViewableDistance) / fadeDistance);
            _n.SetScale(minScale + (_dist / scaleDistance) * (maxScale - minScale));
            _n.SetName(_s.nameplateData.name);
            _n.SetHealthBar(_s.nameplateData.maxHealth, _s.nameplateData.health);
            _n.SetHealthbarVisibility(_s.nameplateData.displayHealth || true);
        }
    }
#endregion

#region Public Functions
    public void TrackSelectable(Selectable _selectable) {
        string _id = _selectable.gameObject.GetInstanceID().ToString();

        if (m_Selectables.ContainsKey(_id)) return; // already tracking
        m_Selectables.Add(_id, _selectable);
    }

    public void ForgetSelectable(Selectable _selectable) {
        string _id = _selectable.gameObject.GetInstanceID().ToString();

        if (!m_Selectables.ContainsKey(_id)) return; // not tracking
        m_Selectables.Remove(_id);
    }
#endregion
}
