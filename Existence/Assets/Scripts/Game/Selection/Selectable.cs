
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
    protected void UpdateNameplate(string _name, int _health, int _maxHealth, bool _displayHealth=false) {
        nameplateData.name = _name;
        nameplateData.health = _health;
        nameplateData.maxHealth = _maxHealth;
        nameplateData.displayHealth = _displayHealth;
        nameplateData.isVisible = true;
    }
#endregion
}
