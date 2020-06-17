
using UnityEngine;

/// <summary>
/// To be derived by objects that are selectable with nameplates
/// </summary>
public class Selectable : GameSystem
{
    public Nameplate nameplate;
    public bool selected;

    protected NameplateData m_NameplateData;

    public NameplateData nameplateData {
        get {
            if (m_NameplateData == null) {
                LogWarning("Trying to return nameplate, but it has not been initialized yet.");
            }
            return m_NameplateData;
        }
    }

#region Protected Functions
    protected void UpdateNameplate(string _name, int _health, int _maxHealth, int _lvl, bool _displayHealth=false) {
        m_NameplateData.name = _name;
        m_NameplateData.health = _health;
        m_NameplateData.maxHealth = _maxHealth;
        m_NameplateData.displayHealth = _displayHealth;
        m_NameplateData.isVisible = true;
        m_NameplateData.lvl = _lvl;
    }
#endregion
}
