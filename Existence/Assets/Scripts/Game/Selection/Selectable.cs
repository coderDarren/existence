
using UnityEngine;

/// <summary>
/// To be derived by objects that are selectable with nameplates
/// </summary>
public class Selectable : GameSystem
{
    public Vector3 nameplateOffset;
    protected NameplateData m_Nameplate;

    public NameplateData nameplate {
        get {
            if (m_Nameplate == null) {
                LogWarning("Trying to return nameplate, but it has not been initialized yet.");
            }
            return m_Nameplate;
        }
    }

#region Protected Functions
    protected void UpdateNameplate(string _name, int _health, int _maxHealth, bool _displayHealth=false) {
        nameplate.name = _name;
        nameplate.health = _health;
        nameplate.maxHealth = _maxHealth;
        nameplate.displayHealth = _displayHealth;
    }
#endregion
}
