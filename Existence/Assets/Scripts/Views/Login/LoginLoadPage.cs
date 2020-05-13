using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;
using UnityCore.Scene;

public class LoginLoadPage : Page
{

    private Session m_Session;
    private LoginController m_Controller;
    private SceneController m_SceneController;
    private PlayerData m_SelectedPlayer;

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

    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                Log("Trying to access session, but no instance was found.");
            }
            return m_Session;
        }
    }

#region Private Functions
    private void OnSceneLoaded(SceneType _scene) {
        if (!session) return;
        if (m_SelectedPlayer == null) {
            Log("Trying to load player, but no player was selected.");
            return;
        }

        if (_scene == SceneType.Game) {
            // subscribe to network connect event to bring down the loading page
            session.network.OnConnect += OnServerConnect;
            // tell the session to connect
            session.StartGame(m_SelectedPlayer);
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
        m_SelectedPlayer = controller.selectedPlayer;
        if (!sceneController) return;
        sceneController.Load(SceneType.Game, OnSceneLoaded);
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        session.network.OnConnect -= OnServerConnect;
    }
#endregion
}
