
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class HUDPage : Page
{
    public Image xpProgress;

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

#region Private Functions
    private void Configure() {
        xpProgress.fillAmount = session.player.XpProgress();
    }

    private void OnPlayerConnected() {
        session.player.OnXpAdded += OnPlayerXpAdded;
        xpProgress.fillAmount = session.player.XpProgress();
    }

    private void OnPlayerDisconnected() {
        session.player.OnXpAdded -= OnPlayerXpAdded;
    }

    private void OnPlayerXpAdded(int _xp) {
        xpProgress.fillAmount = session.player.XpProgress();
    }
#endregion
    
#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        Configure();

        if (session) {
            session.OnPlayerConnected += OnPlayerConnected;
            session.OnPlayerDisconnected += OnPlayerDisconnected;
            session.player.OnXpAdded += OnPlayerXpAdded;
        }
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();

        if (session) {
            session.OnPlayerConnected -= OnPlayerConnected;
            session.OnPlayerDisconnected -= OnPlayerDisconnected;
            session.player.OnXpAdded -= OnPlayerXpAdded;
        }
    }
#endregion
}
