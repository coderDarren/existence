using UniRx.Async;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class AccountCreationPage : Page
{
    public GameObject loadingGraphic;
    public Text statusLabel;
    public InputField firstName;
    public InputField lastName;
    public InputField username;
    public InputField password;
    public InputField passwordConfirm;
    public InputField email;

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
    public async void Create() {
        if (!controller) return;
        AccountData _data = new AccountData();
        _data.first_name = firstName.text;
        _data.last_name = lastName.text;
        _data.username = username.text;
        _data.password = password.text;
        _data.email = email.text;

        loadingGraphic.SetActive(true);
        SetInputActive(false);
        ShowMessage("Creating account. Please wait...");

        int _status = await controller.CreateAccount(_data);

        loadingGraphic.SetActive(false);
        SetInputActive(true);
        ShowMessage(_status == 200 ? "<color=#0f0>Successfully created account.</color>" : "<color=#f00>Failed to create account.</color>");

        Log("Status: "+_status);
    }

    public void GoBack() {
        if (!controller) return;
        //controller.GoToLogin();
    }
#endregion

#region Private Functions
    private void SetInputActive(bool _active) {
        username.interactable = _active;
        password.interactable = _active;
        passwordConfirm.interactable = _active;
        firstName.interactable = _active;
        lastName.interactable = _active;
        email.interactable = _active;
    }

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
