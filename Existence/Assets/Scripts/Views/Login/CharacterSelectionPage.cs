using UniRx.Async;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class CharacterSelectionPage : Page
{
    public GameObject CharacterCard;
    public RectTransform characterCardContainer;
    public Text playerName;
    public Text username;

    private LoginController m_Controller;
    private List<GameObject> m_PlayerCards;

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
    public void SelectCharacter(PlayerData _player) {
        if (!controller) return;
        Log(_player.player.name+" was selected");
        controller.SelectCharacter(_player);
    }

    public void CreateCharacter() {
        if (!controller) return;
        controller.GoToCharacterCreation();
    }

    public void Play() {
        if (!controller) return;
        controller.Play();
    }

    public void SignOut() {
        if (!controller) return;
        controller.GoToLogin();
    }
#endregion

#region Private Functions
    private void ConfigureCharacterScrollView() {
        if (!controller) return;
        DeleteCharacterScrollView();
        foreach (PlayerData _player in controller.session.accountPlayers) {
            GameObject _go = Instantiate(CharacterCard) as GameObject;
            _go.GetComponent<RectTransform>().SetParent(characterCardContainer);
            _go.transform.localScale = Vector3.one;
            CharacterCard _card = _go.GetComponent<CharacterCard>();
            _card.Init(this, _player);
            m_PlayerCards.Add(_go);
        }
        
        Vector2 _size = characterCardContainer.sizeDelta;
        _size.y = m_PlayerCards.Count * 160 + m_PlayerCards.Count * 4;
        characterCardContainer.sizeDelta = _size;
    }

    private void DeleteCharacterScrollView() {
        for (int i = m_PlayerCards.Count - 1; i >= 0; i--) {
            GameObject _go = m_PlayerCards[i];
            m_PlayerCards.RemoveAt(i);
            Destroy(_go);
        }
    }

    private void ShowMessage(string _msg) {
        //controller.ShowMessage(statusLabel, _msg);
    }

    private void StopShowingMessage() {
        //controller.StopShowingMessage();
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        m_PlayerCards = new List<GameObject>();
        ConfigureCharacterScrollView();
        playerName.text = "Hello, " + controller.session.account.first_name;
        username.text = "Signed in as " + controller.session.account.username;
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopShowingMessage();
        DeleteCharacterScrollView();
    }
#endregion
}
