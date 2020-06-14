using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

/// <summary>
/// Hold data about the player
/// </summary>
public class Player : GameSystem
{
    public delegate void IntAction(int _data);
    public event IntAction OnXpAdded;   

    public enum Weapon {oneHandRanged, oneHandMelee, twoHandRanged, twoHandMelee, fist};
    public Weapon weapon;
    public float range;
    
    private PlayerData m_Data;
    private StatData m_GearStats;
    private StatData m_BuffStats;
    private StatData m_TrickleStats;
    private Session m_Session;
    private InventoryPage m_InventoryWindow;
    private int healDelta = 5;
    private float healDeltaTimer = 0;
    private float healDeltaSeconds = 1;

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

    private InventoryPage inventoryWindow {
        get {
            if (!m_InventoryWindow) {
                m_InventoryWindow = InventoryPage.instance;
            }
            return m_InventoryWindow;
        }
    }

#region Unity Functions
    private void Update() {
        HandleHealDelta();
    }
#endregion

#region Public Functions
    /// <summary>
    /// Call from session when network connects
    /// </summary>
    public void ConnectWithData(PlayerData _data) {
        Dispose();
        m_Data = _data;
        m_Data.player.health = MaxHealth();
        m_GearStats = new StatData();
        m_BuffStats = new StatData();
        m_TrickleStats = new StatData();

        if (session && session.network) {
            session.network.OnMobDeath += OnMobDeath;
        } 
    }

    public void Dispose() {
        if (!session) return;
        if (!session.network) return;
        session.network.OnMobDeath -= OnMobDeath;
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

    public int MaxHealth() {
        return m_Data.player.level * 25;
    }

    public void AddInventory(ItemData _item) {
        List<ItemData> _inventory = new List<ItemData>(m_Data.inventory);
        _inventory.Add(_item);
        m_Data.inventory = _inventory.ToArray();

        // redraw inventory if the window is open
        if (inventoryWindow) {
            inventoryWindow.Redraw();
        }
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

    private void HandleHealDelta() {
        healDeltaTimer += Time.deltaTime;
        if (healDeltaTimer >= healDeltaSeconds) {
            m_Data.player.health += healDelta;
            m_Data.player.health = Mathf.Clamp(m_Data.player.health, 0, MaxHealth());
            healDeltaTimer = 0;
        }
    }

    private void OnMobDeath(NetworkMobDeathData _data) {
        foreach (NetworkMobXpAllottment _mxa in _data.xpAllottment) {
            if (m_Data.player.name == _mxa.playerName) {
                AddXp(_mxa.xp);
                break;
            }
        }
    }

    private void TryRunAction(IntAction _action, int _data) {
        try {
            _action(_data);
        } catch (System.Exception _e) {
        }
    }
    
#endregion
}
