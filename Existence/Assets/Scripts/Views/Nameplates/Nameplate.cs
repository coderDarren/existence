
using UnityEngine;
using UnityEngine.UI;

public class NameplateData {
    public string name;
    public int health;
    public int maxHealth;
    public bool displayHealth;
    public NameplateData() {}
    public NameplateData(string _name, int _health, int _maxHealth, bool _displayHealth=true) {
        name = _name;
        health = _health;
        maxHealth = _maxHealth;
    }
}

[RequireComponent(typeof(CanvasGroup))]
public class Nameplate : GameSystem
{
        
    public Text nameLabel;

    private CanvasGroup m_Canvas;

#region Unity Functions
    private void Awake() {
        m_Canvas = GetComponent<CanvasGroup>();
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
#endregion

#region Private Functions

#endregion
}
