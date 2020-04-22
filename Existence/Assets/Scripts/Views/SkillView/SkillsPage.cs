using System.Collections;
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
    private Hashtable m_Pages;
    private Hashtable m_State;

    public StatData skills {
        get {
            return m_Skills;
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

    public void OpenSection(int _section) {
        OpenSection((SkillSection)_section);
    }

    public void OpenSection(SkillSection _section) {
        foreach(DictionaryEntry _entry in m_Pages) {
            var _k = (SkillSection)_entry.Key;
            var _v = (GameObject)_entry.Value;
            _v.SetActive(false);
            if (_k == _section) {
                _v.SetActive(true);
                m_State[_k] = true;
            }
        }
    }

    public bool SectionDidInit(SkillSection _section) {
        return (bool)m_State[_section];
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        loadingGraphic.SetActive(false);

        // page hash
        m_Pages = new Hashtable();
        m_Pages.Add(SkillSection.CORE, coreSection);
        m_Pages.Add(SkillSection.HEALTH, healthSection);
        m_Pages.Add(SkillSection.WEAPONS, weaponsSection);
        m_Pages.Add(SkillSection.COMBAT, combatSection);
        m_Pages.Add(SkillSection.TRADE, tradeSection);
        m_Pages.Add(SkillSection.EXPLORING, exploringSection);

        // state hash
        m_State = new Hashtable();
        m_State.Add(SkillSection.CORE, false);
        m_State.Add(SkillSection.HEALTH, false);
        m_State.Add(SkillSection.WEAPONS, false);
        m_State.Add(SkillSection.COMBAT, false);
        m_State.Add(SkillSection.TRADE, false);
        m_State.Add(SkillSection.EXPLORING, false);

        m_Skills = StatData.Copy(session.playerData.stats);
        OpenSection(SkillSection.CORE);
    }
#endregion
}
