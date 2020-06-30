
using UnityEngine;
using UnityCore.Menu;

public class ViewHotkeyListener : GameSystem
{
    public PageController pageController;
    public KeyCode skillsKey=KeyCode.U;
    public KeyCode hudKey=KeyCode.H;
    public KeyCode inventoryKey=KeyCode.I;
    public KeyCode equipmentKey=KeyCode.C;

    private Chatbox m_Chat;
    private bool m_InputLocked;

    private Chatbox chat {
        get {
            if (!m_Chat) {
                m_Chat = Chatbox.instance;
            }
            if (!m_Chat) {
                LogError("Trying to access chat, but no instance of Chatbox was found.");
            }
            return m_Chat;
        }
    }

#region Unity Functions
    private void OnEnable() {
        if (!chat) return;
        chat.OnChatStarted += OnChatStarted;
        chat.OnChatEnded += OnChatEnded;
    }

    private void OnDisable() {
        if (!chat) return;
        chat.OnChatStarted -= OnChatStarted;
        chat.OnChatEnded -= OnChatEnded;
    }

    private void Update() {
        if (m_InputLocked) return;
        
        if (Input.GetKeyDown(skillsKey)) {
            TogglePage(PageType.Skills);
        }

        if (Input.GetKeyDown(hudKey)) {
            TogglePage(PageType.HUD);
        }

        if (Input.GetKeyDown(inventoryKey)) {
            TogglePage(PageType.Inventory);
        }

        if (Input.GetKeyDown(equipmentKey)) {
            TogglePage(PageType.Equipment);
        }
    }
#endregion

#region Private Functions
    private void TogglePage(PageType _page) {
        if (pageController.PageIsOn(_page)) {
            pageController.TurnPageOff(_page);
        } else {
            pageController.TurnPageOn(_page);
        }
    }

    private void OnChatStarted() {
        m_InputLocked = true;
    }

    private void OnChatEnded() {
        m_InputLocked = false;
    }
#endregion
}
