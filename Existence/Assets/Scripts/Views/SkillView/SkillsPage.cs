
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class SkillsPage : Page
{
    public Button saveButton;

    private Session m_Session;
    private StatData m_Skills;

    public StatData skills {
        get {
            return m_Skills;
        }
    }
    
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

#region Public Functions
    public async void Save() {
        Log("[Save]: Sending request...");
        long _start = NetworkTimestamp.NowMilliseconds();
        saveButton.interactable = false;
        bool _res = await DatabaseService.GetService(debug).UpdateStats(m_Skills);
        saveButton.interactable = true;
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [Save]: "+_res);

        if (_res) {
            session.playerData.stats = StatData.Copy(m_Skills);
        }
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        m_Skills = StatData.Copy(session.playerData.stats);
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        
    }
#endregion
}
