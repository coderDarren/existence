
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Nameplate : GameSystem
{
    public delegate void UpdateDelegate(Nameplate _n, Vector3 _pos);
    public static event UpdateDelegate OnNameplateUpdated;

    public Text nameLabel;
    public Text nameLabelShadow;
    public Image healthBar;
    public CanvasGroup healthBarCanvas;
    public LayerMask foregroundLayer;
    public LayerMask backgroundLayer;

    private Canvas m_ParentCanvas;
    private CanvasGroup m_Canvas;
    public Vector3 target;
    public Vector3 offset;

#region Unity Functions
    private void Awake() {
        m_ParentCanvas = transform.parent.GetComponent<Canvas>();
        m_Canvas = GetComponent<CanvasGroup>();
    }

    private void OnDisable() {
    }
#endregion

#region Public Functions
    public void SetAlpha(float _alpha) {
        m_Canvas.alpha = _alpha;
    }

    public void SetScale(float _scale) {
        transform.localScale = Vector3.one * _scale;
    }

    public void SetName(string _name) {
        nameLabel.text = _name;
        nameLabelShadow.text = _name;
    }

    public void SetHealthBar(int _max, int _curr) {
        if (!healthBar) return;
        healthBar.fillAmount = _curr / (float)_max;
    }

    public void SetHealthbarVisibility(bool _visible) {
        if (!healthBarCanvas) return;
        healthBarCanvas.alpha = _visible ? 1 : 0;
    }

    public void BringToForeground() {
        //m_ParentCanvas.sortingOrder = 1000;
        transform.parent.gameObject.layer = LayerMask.NameToLayer("UI_Secondary");
    }

    public void PushToBackground() {
        //m_ParentCanvas.sortingOrder = 0;
        transform.parent.gameObject.layer = LayerMask.NameToLayer("Default");
    }
#endregion

#region Private Functions
    private void TryAction(UpdateDelegate _action, Nameplate _n, Vector3 _pos) {
        try {
            _action(_n, _pos);
        } catch (System.Exception _e) {}
    }
#endregion
}
