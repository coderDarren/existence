
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityCore.Menu;
using ProScripts;

public class DualOptionMessagePage : Page
{
    public static DualOptionMessagePage instance;

    public Text title;
    public Text message;
    public Text option1Label;
    public Text option2Label;
    public ProButton option1Button;
    public ProButton option2Button;

    private UnityAction m_Option1Action;
    private UnityAction m_Option2Action;

#region Public Functions
    public void Redraw(string _title, string _message, string _option1Label, string _option2Label) {
        title.text = _title;
        message.text = _message;
        option1Label.text = _option1Label;
        option2Label.text = _option2Label;
    }

    public void OnOption1(UnityAction _action) {
        m_Option1Action = _action;
        option1Button.upSettings.eventAction.AddListener(m_Option1Action);
    }

    public void OnOption2(UnityAction _action) {
        m_Option2Action = _action;
        option2Button.upSettings.eventAction.AddListener(m_Option2Action);
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (instance != null) return;
        if (!instance) {
            instance = this;
        }
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        instance = null;
        option1Button.upSettings.eventAction.RemoveListener(m_Option1Action);
        option2Button.upSettings.eventAction.RemoveListener(m_Option2Action);
    }
#endregion
}
