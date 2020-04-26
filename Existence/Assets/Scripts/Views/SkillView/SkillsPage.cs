using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class SkillsPage : Page
{
    public Text statPointLabel;
    public Button saveButton;
    public GameObject coreSection;
    public GameObject healthSection;
    public GameObject weaponsSection;
    public GameObject combatSection;
    public GameObject tradeSection;
    public GameObject exploringSection;
    public GameObject loadingGraphic;

    private Session m_Session;
    private Hashtable m_Stats;
    private Hashtable m_OtherStats;
    private Hashtable m_Pages;
    private int m_StatPoints;

    public Hashtable stats {
        get {
            return m_Stats;
        }
    }

    public Hashtable otherStats {
        get {
            return m_OtherStats;
        }
    }

    public int statPoints {
        get {
            return m_StatPoints;
        }
        set {
            m_StatPoints = value;
            statPointLabel.text = "Stat Points: "+m_StatPoints;
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
        StatData _updatedStats = StatData.FromHashtable(m_Stats);
        bool _res = await DatabaseService.GetService(debug).UpdateStats(_updatedStats);
        loadingGraphic.SetActive(false);
        saveButton.interactable = true;
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [Save]: "+_res);

        if (_res) {
            session.player.SaveBaselineStats(_updatedStats);
            session.player.data.player.statPoints = m_StatPoints;
            _res = await DatabaseService.GetService(debug).UpdatePlayer(session.player.data.player);
            if (!_res) {
                LogError("Failed to save stat points after allocating stat points.");
            }
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
            }
        }
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

        statPoints = session.player.data.player.statPoints;
        m_Stats = session.playerData.stats.ToHashtable();
        m_OtherStats = session.player.buffStats.Combine(session.player.gearStats).ToHashtable();
        OpenSection(SkillSection.CORE);
    }
#endregion
}
