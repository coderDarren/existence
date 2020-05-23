using UniRx.Async;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class CharacterCreationPage : Page
{   
    
    public GameObject loadingGraphic;
    public Text statusLabel;
    public InputField name;
    public Text playerName;
    public Text username;

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

    public async void CreateCharacter() {
        if (!FieldsAreValid()) return;
        if (!controller) return;

        loadingGraphic.SetActive(true);
        ShowMessage("Creating player. Please wait...");

        CreatePlayerRequest _req = new CreatePlayerRequest(name.text, controller.session.account.id, controller.session.account.apiKey);
        int _status = await controller.CreateCharacter(_req);
        Log("status: "+_status);
        loadingGraphic.SetActive(false);

        switch (_status) {
            case 1398:
            case 1399:
                ShowMessage("Failed to authenticate request. Please try again or contact support.");
                break;
            case 1400:
                ShowMessage("This name is already taken. Please use a different name.");
                break;
            case 200:
                ShowMessage("Successfully created new player.");
                await UniTask.Delay(1000);
                controller.Play();
                break;
            default:
                ShowMessage("Something went wrong. Please try again or contact support.");
                break;
        }
    }

    public void GoBack() {
        if (!controller) return;
        controller.GoToCharacterSelection();
    }
#endregion

#region Private Functions
    private bool FieldsAreValid() {
        if (name.text.Length <= 5) {
            ShowMessage("Your name must be at least 6 characters long");
            return false;
        }
        return true;
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
        name.text = string.Empty;
        playerName.text = "Hello, " + controller.session.account.first_name;
        username.text = "Signed in as " + controller.session.account.username;
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopShowingMessage();
    }
#endregion
}
