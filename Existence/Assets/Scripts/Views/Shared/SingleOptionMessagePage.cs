
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityCore.Menu;
using ProScripts;

public class SingleOptionMessagePage : Page
{
    public static SingleOptionMessagePage instance;

    public Text title;
    public Text message;
    public Text optionLabel;
    public ProButton optionButton;

    private UnityAction m_OptionAction;

#region Public Functions
    public void Redraw(string _title, string _message, string _optionLabel) {
        title.text = _title;
        message.text = _message;
        optionLabel.text = _optionLabel;
    }

    public void OnOption(UnityAction _action) {
        m_OptionAction = _action;
        optionButton.upSettings.eventAction.AddListener(_action);
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
        optionButton.upSettings.eventAction.RemoveListener(m_OptionAction);
    }
#endregion
}
