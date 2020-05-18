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
        if (!FieldsAreValid()) return;
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

        switch (_status) {
            case 1400: ShowMessage("<color=#f00>Username must be at least 5 characters</color>"); break;
            case 1401: ShowMessage("<color=#f00>Username cannot begin with a number or special character</color>"); break;
            case 1402: ShowMessage("<color=#f00>Password must be at least 8 characters</color>"); break;
            case 1403: ShowMessage("<color=#f00>Password cannot begin with a special character or a number</color>"); break;
            case 1404: ShowMessage("<color=#f00>First name cannot contain special characters or numbers</color>"); break;
            case 1405: ShowMessage("<color=#f00>Last name cannot contain special characters or numbers</color>"); break;
            case 1406: ShowMessage("<color=#f00>Email is not supported</color>"); break;
            case 1407: ShowMessage("<color=#f00>Account already exists for username</color>"); break;
            case 1408: ShowMessage("<color=#f00>Account already exists for email</color>"); break;
            case 200: 
                ShowMessage("<color=#0f0>Successfully created account</color>"); 
                await UniTask.Delay(1000);
                GoBack();
                break;
            default: ShowMessage("<color=#f00>Failed to create account for unknown reason. Please contact support</color>"); break;
        }

        Log("Status: "+_status);
    }

    public void GoBack() {
        if (!controller) return;
        controller.GoToLogin();
    }
#endregion

#region Private Functions
    private bool FieldsAreValid() {
        if (username.text.Length == 0 ||
            password.text.Length == 0 ||
            passwordConfirm.text.Length == 0 ||
            firstName.text.Length == 0 ||
            lastName.text.Length == 0 ||
            email.text.Length == 0) {
            ShowMessage("<color=#f00>One or more fields are empty. Please fill out all fields</color>");
            return false;
        }

        if (!password.text.Equals(passwordConfirm.text)) {
            ShowMessage("<color=#f00>Passwords do not match</color>");
            return false;
        }

        return true;
    }

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
        username.text = string.Empty;
        password.text = string.Empty;
        passwordConfirm.text = string.Empty;
        firstName.text = string.Empty;
        lastName.text = string.Empty;
        email.text = string.Empty;
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopShowingMessage();
    }
#endregion
}
