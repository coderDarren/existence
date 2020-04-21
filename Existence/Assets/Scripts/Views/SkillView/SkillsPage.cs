
using UnityEngine;
using UnityCore.Menu;

public class SkillsPage : Page
{
    private Session m_Session;
    
    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();

    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();

    }
#endregion
}
