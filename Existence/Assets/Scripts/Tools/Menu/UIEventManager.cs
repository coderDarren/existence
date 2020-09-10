
public class UIEventManager 
{
    private static UIEventManager m_Instance;
    private int m_HoverStack;
    private bool m_Hovering;

    public static UIEventManager Instance() {
        if (m_Instance == null) {
            m_Instance = new UIEventManager();
        }
        return m_Instance;
    }

    public bool hovering {
        get {
            return m_Hovering;
        }
    }

    public UIEventManager() {
        m_HoverStack = 0;
        m_Hovering = false;
    }

    public void Hover() {
        m_HoverStack ++;
        m_Hovering = true;
        Log("Hover stack: "+m_HoverStack);
    }

    public void Exit() {
        m_HoverStack --;
        if (m_HoverStack <= 0) {
            m_HoverStack = 0;
            m_Hovering = false;
        }
        Log("Hover stack: "+m_HoverStack);
    }

    private void Log(string _msg) {
        UnityEngine.Debug.Log("[UIEventManager] "+_msg);
    }
}
