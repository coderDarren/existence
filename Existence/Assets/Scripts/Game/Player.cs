
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hold data about the player
/// </summary>
public class Player : GameSystem
{
    public Text nameLabel;
    
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
    private void Awake() {
        // start with default name
        m_Data = new PlayerData();
        m_Data.player = new PlayerInfo();
        m_Data.stats = new StatData();
        m_Data.player.name = RandomString(12);
        m_GearStats = new StatData();
        m_BuffStats = new StatData();
        m_TrickleStats = new StatData();
        nameLabel.text = m_Data.player.name;
    }
#endregion

#region Public Functions
    /// <summary>
    /// Call from session when network connects
    /// </summary>
    public void ConnectWithData(PlayerData _data) {
        m_Data = _data;
        nameLabel.text = _data.player.name;
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
#endregion

#region Private Functions
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
#endregion
}
