
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : GameSystem
{
    public Text name;
    public Text profession;
    public Text location;
    public Text level;
    public Image xp;

    private CharacterSelectionPage m_Controller;
    private PlayerData m_Player;

#region Public Functions
    public void Init(CharacterSelectionPage _controller, PlayerData _player) {
        m_Controller = _controller;
        m_Player = _player;
        name.text = m_Player.player.name;
        level.text = "LV "+m_Player.player.level;
        xp.fillAmount = m_Player.player.xp / MaxXp();
    }

    public void OnSelect() {
        m_Controller.SelectCharacter(m_Player);
    }
#endregion

#region Private Functions
    private float MaxXp() {
        return m_Player.player.level * 500;
    }
#endregion
}
