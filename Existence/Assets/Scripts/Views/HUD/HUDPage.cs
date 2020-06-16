
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class HUDPage : Page
{
    public Image xpProgress;
    public Image hpProgress;
    public Text hpLabel;
    public Text nameLabel;
    public Text infoLabel;

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

#region Unity Functions
    private void Update() {
        if (!session || session.player == null) return;
        hpProgress.fillAmount = session.player.HpProgress();
        hpLabel.text = session.player.data.player.health + "/" +session.player.MaxHealth();
    }
#endregion

#region Private Functions
    private void Configure() {
        xpProgress.fillAmount = session.player.XpProgress();
    }

    private void OnPlayerConnected() {
        session.player.OnXpAdded += OnPlayerXpAdded;
        xpProgress.fillAmount = session.player.XpProgress();
        UpdateShadowedText(nameLabel, session.player.data.player.name);
        UpdateShadowedText(infoLabel, "LV. " + session.player.data.player.level + " Soldier");
    }

    private void OnPlayerDisconnected() {
        session.player.OnXpAdded -= OnPlayerXpAdded;
    }

    private void OnPlayerXpAdded(int _xp) {
        xpProgress.fillAmount = session.player.XpProgress();
    }

    private void UpdateShadowedText(Text _t, string _content) {
        Text[] _texts = _t.GetComponentsInChildren<Text>();
        foreach (Text _text in _texts) {
            _text.text = _content;
        }
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
