
using UnityEngine;
using UnityCore.Menu;

/*
 * For now this just needs to respond to shop terminal events..
 * ..more to come surely..
 */
public class ShopManager : GameSystem
{
    public static ShopManager instance;

    public PageController pageController;

    private Session m_Session;

    // Use this to prevent opening multiple shop terminals
    private bool m_UsingShopTerminal;
    private NetworkShopTerminalData m_ShopTerminalData;

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

    public NetworkShopTerminalData shopTerminalData {
        get {
            return m_ShopTerminalData;
        }
    }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        if (instance != this) return;

        if (!session) return;
        if (!session.network) return;
        session.network.OnShopTerminalInteracted += OnShopTerminalInteracted;
    }

    private void OnDisable() {
        if (instance != this) return;
        instance = null;

        if (!session) return;
        if (!session.network) return;
        session.network.OnShopTerminalInteracted -= OnShopTerminalInteracted;
    }
#endregion

#region Public Functions
    public void OpenShop(int _id) {
        if (!session) return;
        if (!session.network) return;
        if (m_UsingShopTerminal) return;
        session.network.InteractShop(new NetworkShopTerminalInteractData(_id));
    }

    public void CloseShop() {
        m_UsingShopTerminal = false;
        m_ShopTerminalData = null;
        pageController.TurnPageOff(PageType.ShopTerminal);
    }
#endregion

#region Private Functions
    private void OnShopTerminalInteracted(NetworkShopTerminalData _data) {
        m_UsingShopTerminal = true;
        m_ShopTerminalData = _data;
        pageController.TurnPageOn(PageType.ShopTerminal);
    }
#endregion
}
