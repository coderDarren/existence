using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;
using UniRx.Async;

public class LoginController : GameSystem
{
    public static LoginController instance;

    // Create a reference to the local scene menu
    public PageController localMenu;

    // Add another reference to the persistent cross-scene menu
    private PageController m_Menu;
    private Session m_Session;
    private IEnumerator m_MessageJob;
    private PlayerData m_SelectedPlayer;

    private PageController menu {
        get {
            if (!m_Menu) {
                m_Menu = PageController.instance;
            }
            if (!m_Menu) {
                Log("Trying to access persistent menu, but no instance was found.");
            }
            return m_Menu;
        }
    }

    public Session session {
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

    public PlayerData selectedPlayer {
        get {
            return m_SelectedPlayer;
        }
    }

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void OnDisable() {
        StopShowingMessage();
    }
#endregion

#region Public Functions
    public async UniTask<bool> Login(string _un, string _pass) {
        if (!session) return false;
        AccountData _account = await DatabaseService.GetService(true).Authenticate(new AuthenticationData(_un, _pass));
        if (_account != null) {
            session.InitAccount(_account);
        }
        return _account != null;
    }

    public void GoToLogin() {
        session.SignOut();
        localMenu.TurnPageOff(PageType.CharacterSelection, PageType.Login, false);
    }

    public void GoToAccountCreation() {
        
    }

    public async UniTask<bool> GoToCharacterSelection() {
        // if session does not already have character info
        if (!session) return false;
        if (session.account == null) {
            LogError("Trying to go to character selection, but no account was found in the session.");
            return false;
        }

        if (session.accountPlayers == null) {
            PlayerData[] _accountPlayers = await DatabaseService.GetService(true).GetAccountPlayers(session.account);
            if (_accountPlayers != null) {
                session.InitAccountPlayers(_accountPlayers);
                localMenu.TurnPageOff(PageType.Login, PageType.CharacterSelection, false);
                return true;
            } else {
                Log("Could not load players from the account.");
                return false;
            }
        }

        localMenu.TurnPageOff(PageType.Login, PageType.CharacterSelection, false);

        return true;
    }

    public void GoToCharacterCreation() {

    }

    public void SelectCharacter(PlayerData _player) {
        m_SelectedPlayer = _player;
    }

    public void Play() {
        if (!menu) return;
        if (m_SelectedPlayer == null) return;
        menu.TurnPageOn(PageType.LoginLoad);
    }

    public void ShowMessage(Text _label, string _msg) {
        StopShowingMessage();
        m_MessageJob = RunShowMessage(_label, _msg);
        StartCoroutine(m_MessageJob);
    }

    public void StopShowingMessage() {
        if (m_MessageJob != null) {
            StopCoroutine(m_MessageJob);
        }
    }
#endregion

#region Private Functions
    private IEnumerator RunShowMessage(Text _label, string _msg) {
        CanvasGroup _canvas = _label.GetComponent<CanvasGroup>();
        _canvas.alpha = 0;
        float _initAlpha = 0;
        float _targetAlpha = 1;
        float _timer = 0;
        float _duration = 0.25f;

        _label.text = _msg;

        while (_timer <= _duration) {
            _canvas.alpha = Mathf.Lerp(_initAlpha, _targetAlpha, _timer / _duration);
            _timer += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(5);

        _timer = 0;
        _initAlpha = 1;
        _targetAlpha = 0;

        while (_timer <= _duration) {
            _canvas.alpha = Mathf.Lerp(_initAlpha, _targetAlpha, _timer / _duration);
            _timer += Time.deltaTime;
            yield return null;
        }

        _label.text = string.Empty;
    }
#endregion
}
