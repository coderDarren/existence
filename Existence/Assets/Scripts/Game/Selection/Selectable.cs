
using UnityEngine;

/// <summary>
/// To be derived by objects that are selectable with nameplates
/// </summary>
public class Selectable : GameSystem
{
    public Nameplate nameplate;
    public bool useNameplate=true;

    protected NameplateData m_NameplateData;

    private bool m_Selected;

    public NameplateData nameplateData {
        get {
            if (m_NameplateData == null) {
                LogWarning("Trying to return nameplate, but it has not been initialized yet.");
            }
            return m_NameplateData;
        }
    }

    public bool selected {
        get {
            return m_Selected;
        }
    }

#region Protected Functions
    protected void UpdateNameplate(string _name, int _health, int _maxHealth, int _lvl, bool _displayHealth=false) {
        if (!nameplate || m_NameplateData == null) return;
        m_NameplateData.name = _name;
        m_NameplateData.health = _health;
        m_NameplateData.maxHealth = _maxHealth;
        m_NameplateData.displayHealth = _displayHealth;
        m_NameplateData.isVisible = true;
        m_NameplateData.lvl = _lvl;
    }
#endregion

#region Overridable Functions
    // These functions invoked by NameplateController..
    // ..during target selection events
    // Override these in children for custom behaviors
    public virtual void OnSelected() {
        m_Selected = true;
    }
    public virtual void OnDeselected() {
        m_Selected = false;
    }
#endregion 
}
