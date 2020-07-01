﻿
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
    
    public EquipmentWindow[] windows;

    private Session m_Session;
    private PlayerData m_PlayerData;
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

#endregion

#region Private Functions
    private void Redraw() {
        windows[m_ActiveWindow].EraseWindow();
        windows[m_ActiveWindow].DrawWindow();
    }
#endregion
    
#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (session.playerData == null) return;
        
        m_PlayerData = session.playerData;
        windows[m_ActiveWindow].InitWindow(this, m_PlayerData);
        Redraw();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        windows[m_ActiveWindow].DisposeWindow();
    }
#endregion
}
