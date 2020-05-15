
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Nameplate : GameSystem
{
    public delegate void UpdateDelegate(Nameplate _n, Vector3 _pos);
    public static event UpdateDelegate OnNameplateUpdated;

    public Text nameLabel;
    public Image healthBar;

    private CanvasGroup m_Canvas;
    public Vector3 target;
    public Vector3 offset;

#region Unity Functions
    private void Awake() {
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
    }

    public void SetPos(Vector3 _pos) {
        transform.position = _pos;
    }

    public void SetHealthBar(int _max, int _curr) {
        healthBar.fillAmount = _curr / (float)_max;
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
