
using UnityEngine;
using UnityCore.Menu;

/*
 * We will use this page controller to manage various equipment tabs such as..
 * 1. Gear
 * 2. Prosthetics
 * 3. Augments
 */
public class EquipmentPage : Page
{
    public static EquipmentPage instance;
    
    public CanvasGroup[] menuButtons;
    public EquipmentWindow[] windows;

    private Session m_Session;
    private int m_ActiveWindow;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Public Functions
    public void Redraw() {
        OpenPage(m_ActiveWindow);
    }

    public void UnequipItem(IItem _item) {
        if (!session) return;
        session.player.NetworkUnequip(_item);
    }

    public void OpenPage(int _index) {
        if (windows[m_ActiveWindow].gameObject.activeSelf) {
            windows[m_ActiveWindow].EraseWindow();
            windows[m_ActiveWindow].gameObject.SetActive(false);
            menuButtons[m_ActiveWindow].alpha = 0.5f;
        }

        m_ActiveWindow = _index;
        windows[m_ActiveWindow].gameObject.SetActive(true);
        windows[m_ActiveWindow].InitWindow(this, session.playerData);
        windows[m_ActiveWindow].EraseWindow();
        windows[m_ActiveWindow].DrawWindow();
        menuButtons[m_ActiveWindow].alpha = 1.0f;
    }
#endregion
    
#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (session.playerData == null) return;
        if (!instance) {
            instance = this;
        }
        
        Redraw();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        windows[m_ActiveWindow].DisposeWindow();
        instance = null;
    }
#endregion
}
