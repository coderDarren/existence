using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;
using UnityCore.Scene;

public class LoginLoadPage : Page
{

    private LoginController m_Controller;
    private SceneController m_SceneController;

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

    private SceneController sceneController {
        get {
            if (!m_SceneController) {
                m_SceneController = SceneController.instance;
            }
            if (!m_SceneController) {
                Log("Trying to access SceneController, but no instance was found.");
            }
            return m_SceneController;
        }
    }

#region Private Functions
    private void OnSceneLoaded(SceneType _scene) {
        if (_scene == SceneType.Game) {
            // subscribe to network connect event to bring down the loading page
            controller.session.network.OnConnect += OnServerConnect;
            // tell the session to connect
            controller.session.ConnectPlayer(controller.selectedPlayer);
        }
    }

    private void OnServerConnect() {
        PageController.instance.TurnPageOff(PageType.LoginLoad);
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
    }

    protected override void OnPageEnter() {
        base.OnPageEnter();
        // start loading scene
        if (!sceneController) return;
        sceneController.Load(SceneType.Game, OnSceneLoaded);
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        controller.session.network.OnConnect -= OnServerConnect;
    }
#endregion
}
