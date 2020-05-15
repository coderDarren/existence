using UniRx.Async;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class AccountCreationPage : Page
{
    public Text statusLabel;
    public InputField firstName;
    public InputField lastName;
    public InputField username;
    public InputField password;
    public InputField passwordConfirm;

    private LoginController m_Controller;

    private LoginController controller {
        get {
            if (!m_Controller) {
                m_Controller = LoginController.instance;
            }
            if (!m_Controller) {
                Log("Trying to access LoginController, but no instance was found.");
            }
            return m_Controller;
        }
    }

#region Public Functions
    public void Create() {
        if (!controller) return;
        //controller.Play();
    }

    public void GoBack() {
        if (!controller) return;
        //controller.GoToLogin();
    }
#endregion

#region Private Functions
   

    private void ShowMessage(string _msg) {
        controller.ShowMessage(statusLabel, _msg);
    }

    private void StopShowingMessage() {
        //controller.StopShowingMessage();
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopShowingMessage();
    }
#endregion
}
