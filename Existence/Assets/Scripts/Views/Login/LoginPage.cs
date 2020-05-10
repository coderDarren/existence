﻿using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class LoginPage : Page
{
    public GameObject loadingGraphic;
    public Text statusLabel;
    public InputField username;
    public InputField password;

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

#region Unity Functions
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && username.interactable) {
            Login();
        }
    }
#endregion

#region Private Functions
    private async void Login() {
        if (!controller) return;

        loadingGraphic.SetActive(true);
        SetInputActive(false);
        ShowMessage("Signing in. Please wait...");

        bool _success = await controller.Login(username.text, password.text);
        
        loadingGraphic.SetActive(false);
        SetInputActive(true);
        ShowMessage(_success ? "<color=#0f0>Successfully logged in.</color>" : "<color=#f00>Failed to log in.</color>");

        if (_success) {
            await UniTask.Delay(1000);
            ShowMessage("Retrieving character list...");
            _success = await controller.GoToCharacterSelection();
            if (!_success) {
                ShowMessage("<color=#f00>Failed to load character list.</color> Try signing in again.");
            }
        }
    }

    private void SetInputActive(bool _active) {
        username.interactable = _active;
        password.interactable = _active;
    }

    private void ShowMessage(string _msg) {
        controller.ShowMessage(statusLabel, _msg);
    }

    private void StopShowingMessage() {
        controller.StopShowingMessage();
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        username.text = string.Empty;
        password.text = string.Empty;
        statusLabel.text = string.Empty;
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopShowingMessage();
    }
#endregion
}
