
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class HUDPage : Page
{
    [Header("Player HUD")]
    public Image xpProgress;
    public Image hpProgress;
    public Text hpLabel;
    public Text nameLabel;
    public Text infoLabel;

    [Header("Target HUDs")]
    public HUDTarget otherTarget;
    public HUDTarget mobTarget;

    private Session m_Session;
    private TargetController m_Targets;

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

    private TargetController targets {
        get {
            if (!m_Targets) {
                m_Targets = TargetController.instance;
            }
            if (!m_Targets) {
                LogWarning("Trying to access targets, but no instance of TargetController was found.");
            }
            return m_Targets;
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
        mobTarget.gameObject.SetActive(false);
        otherTarget.gameObject.SetActive(false);
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

    private void OnTargetSelected(Selectable _s, bool _primary) {
        if (_s == targets.primaryTarget) {
            mobTarget.gameObject.SetActive(true);
            mobTarget.Init(_s.nameplateData);
        } else if (_s == targets.otherTarget) {
            otherTarget.gameObject.SetActive(true);
            otherTarget.Init(_s.nameplateData);
        }
    }

    private void OnTargetDeselected(Selectable _s, bool _primary) {
        if (_s == targets.primaryTarget) {
            mobTarget.gameObject.SetActive(false);
        } else if (_s == targets.otherTarget) {
            otherTarget.gameObject.SetActive(false);
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

        if (targets) {
            targets.OnTargetSelected += OnTargetSelected;
            targets.OnTargetDeselected += OnTargetDeselected;
        }
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();

        if (session) {
            session.OnPlayerConnected -= OnPlayerConnected;
            session.OnPlayerDisconnected -= OnPlayerDisconnected;
            session.player.OnXpAdded -= OnPlayerXpAdded;
        }
        
        if (targets) {
            targets.OnTargetSelected -= OnTargetSelected;
            targets.OnTargetDeselected -= OnTargetDeselected;
        }
    }
#endregion
}
