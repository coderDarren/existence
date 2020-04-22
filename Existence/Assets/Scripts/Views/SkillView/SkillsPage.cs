
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class SkillsPage : Page
{
    public Button saveButton;
    public GameObject coreSection;
    public GameObject healthSection;
    public GameObject weaponsSection;
    public GameObject combatSection;
    public GameObject tradeSection;
    public GameObject exploringSection;
    public GameObject loadingGraphic;

    private Session m_Session;
    private StatData m_Skills;
    private GameObject[] pages;
    private bool m_DidInit;

    public StatData skills {
        get {
            return m_Skills;
        }
    }

    public bool didInit {
        get {
            return m_DidInit;
        }
    }
    
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
    public async void Save() {
        Log("[Save]: Sending request...");
        long _start = NetworkTimestamp.NowMilliseconds();
        saveButton.interactable = false;
        loadingGraphic.SetActive(true);
        bool _res = await DatabaseService.GetService(debug).UpdateStats(m_Skills);
        loadingGraphic.SetActive(false);
        saveButton.interactable = true;
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [Save]: "+_res);

        if (_res) {
            session.playerData.stats = StatData.Copy(m_Skills);
        }
    }

    public void OpenPage(int _index) {
        for (int i = 0; i < pages.Length; i++) {
            pages[i].SetActive(false);
        }
        pages[_index].SetActive(true);
    }
#endregion

#region Override Functions
    protected override void OnPageEnter() {
        base.OnPageEnter();
        m_DidInit = true;
    }

    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (m_DidInit) return;
        loadingGraphic.SetActive(false);
        pages = new GameObject[]{
            coreSection,
            healthSection,
            weaponsSection,
            combatSection,
            tradeSection,
            exploringSection
        };
        m_Skills = StatData.Copy(session.playerData.stats);
        OpenPage(0);
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        m_DidInit = false;
    }
#endregion
}
