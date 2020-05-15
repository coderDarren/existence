
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hold data about the player
/// </summary>
public class Player : GameSystem
{
    public delegate void IntAction(int _data);
    public event IntAction OnXpAdded;   

    public Text nameLabel;
    public enum Weapon {oneHandRanged, oneHandMelee, twoHandRanged, twoHandMelee, fist};
    public Weapon weapon;
    
    private PlayerData m_Data;
    private StatData m_GearStats;
    private StatData m_BuffStats;
    private StatData m_TrickleStats;
    private Session m_Session;

    public PlayerData data {
        get {
            return m_Data;
        }
        set {
            m_Data = value;
        }
    }

    public StatData gearStats { 
        get {
            return m_GearStats;
        }
    }

    public StatData buffStats {
        get {
            return m_BuffStats;
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

#region Unity Functions
#endregion

#region Public Functions
    /// <summary>
    /// Call from session when network connects
    /// </summary>
    public void ConnectWithData(PlayerData _data) {
        m_Data = _data;
        m_GearStats = new StatData();
        m_BuffStats = new StatData();
        m_TrickleStats = new StatData();        
        //nameLabel.text = m_Data.player.name;
    }

    public void SaveBaselineStats(StatData _stats) {
        m_Data.stats = StatData.Copy(_stats);
    }

    /// <summary>
    /// Totals stats from all sources and returns them
    /// Sources:
    ///     - Baseline (stored in player data)
    ///     - Buffs
    ///     - Gear
    ///     - Trickle
    /// </summary>
    public StatData GetAggregatedStats() {
        StatData _out = m_Data.stats.Combine(m_GearStats).Combine(m_BuffStats);
        m_TrickleStats = StatData.TrickleFrom(_out);
        return _out.Combine(m_TrickleStats);
    }

    public async void AddXp(int _xp) {
        m_Data.player.xp += _xp;
        float _max = MaxXp();
        while (m_Data.player.xp >= _max) {
            m_Data.player.level ++;
            m_Data.player.xp = m_Data.player.xp - (int)_max;
            m_Data.player.statPoints += StatPointReward();
            _max = MaxXp();
        }
        bool _success = await DatabaseService.GetService(debug).UpdatePlayer(m_Data.player);
        if (_success) {
            TryRunAction(OnXpAdded, _xp);
        }
    }

    public float XpProgress() {
        return m_Data.player.xp / MaxXp();
    }
#endregion

#region Private Functions
    /// <summary>
    /// Simple linear calculation using a coefficient of 500
    /// </summary>
    private float MaxXp() {
        return m_Data.player.level * 500;
    }

    /// <summary>
    /// Increase stat point allotment every 10 levels
    /// </summary>
    private int StatPointReward() {
        int _factor = (m_Data.player.level / 10) + 1;
        return _factor * 30;
    }

    private string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0,chars.Length)];
        }

        return new string(stringChars);
    }

    private void TryRunAction(IntAction _action, int _data) {
        try {
            _action(_data);
        } catch (System.Exception) {}
    }
    
#endregion
}
