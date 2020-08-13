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

    private TargetController m_TargetController;
    private Hashtable m_Selectables;

    private TargetController targetController {
        get {
            if (!m_TargetController) {
                m_TargetController = TargetController.instance;
            }
            if (!m_TargetController) {
                LogWarning("Trying to access targets, but no instance of TargetController was found.");
            }
            return m_TargetController;
        }
    }

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
            m_Selectables = new Hashtable();
        }
    }

    private void Start() {
        if (targetController) {
            targetController.OnTargetSelected += OnTargetSelected;
            targetController.OnTargetDeselected += OnTargetDeselected;
        }
    }

    private void OnDisable() {
        if (targetController) {
            targetController.OnTargetSelected -= OnTargetSelected;
            targetController.OnTargetDeselected -= OnTargetDeselected;
        }
    }

    private void Update() {
        if (instance != this) {
            LogError("Too many NameplateControllers are in your scene. Remove from "+gameObject.name+" or "+instance.gameObject.name);
            return;
        }

        foreach(DictionaryEntry _entry in m_Selectables) {
            Selectable _s = (Selectable)_entry.Value;
            Nameplate _n = _s.nameplate;
            
            float _dist = Vector3.Distance(Camera.main.transform.position, _s.transform.position);
            if (!_s.selected) {
                _n.SetAlpha(0.15f - (_dist - maxViewableDistance) / fadeDistance);
                _n.SetHealthbarVisibility(_s.nameplateData.displayHealth);
                _n.PushToBackground();
            }
            _n.SetScale(minScale + (_dist / scaleDistance) * (maxScale - minScale));
            _n.SetName(_s.nameplateData.name);
            _n.SetHealthBar(_s.nameplateData.maxHealth, _s.nameplateData.health);
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

#region Private Functions
    private void OnTargetSelected(Selectable _s, bool _primary) {
        Nameplate _n = _s.nameplate;
        if (_s.GetType().IsAssignableFrom(typeof(Mob))) {
            _n.SetAlpha(_primary ? 1 : 0.5f);
        } else {
            _n.SetAlpha(1);
        }
        _n.SetHealthbarVisibility(true);
        _n.BringToForeground();
        _s.selected = true;
    }
    
    private void OnTargetDeselected(Selectable _s, bool _primary) {
        Nameplate _n = _s.nameplate;
        _n.PushToBackground();
        _s.selected = false;
    }
#endregion
}
