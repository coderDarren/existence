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
    private EquipmentPage m_EquipmentWindow;
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

    private EquipmentPage equipmentWindow {
        get {
            if (!m_EquipmentWindow) {
                m_EquipmentWindow = EquipmentPage.instance;
            }
            return m_EquipmentWindow;
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
        InitializeStats();
        InitializeInventory();
        InitializeEquipment();
        //InitializeTestEquipment();

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

    public float HpProgress() {
        return m_Data.player.health / (float)MaxHealth();
    }

    public int MaxHealth() {
        return m_Data.player.level * 25;
    }

    public void AddInventory(IItem _item) {
        m_Data.inventory.Add(_item);

        // redraw inventory if the window is open
        if (inventoryWindow) {
            inventoryWindow.Redraw();
        }
    }

    public void EquipItem(int _id, int _loc) {
        for (int i = m_Data.inventory.Count - 1; i >= 0; i--) {
            if (m_Data.inventory[i].def.id == _id && m_Data.inventory[i].def.slotLoc == _loc) {
                switch (m_Data.inventory[i].def.itemType) {
                    case ItemType.WEAPON: m_Data.equipment.weapons.Add((WeaponItemData)m_Data.inventory[i]); break;
                    case ItemType.ARMOR: m_Data.equipment.armor.Add((ArmorItemData)m_Data.inventory[i]); break;
                    default: break;
                }
                m_Data.inventory.RemoveAt(i);
                break;
            }
        }

        // redraw inventory if the window is open
        if (inventoryWindow) {
            inventoryWindow.Redraw();
        }

        // redraw equipment if the window is open
        if (equipmentWindow) {
            equipmentWindow.Redraw();
        }
    }

    public void UnequipItem(int _id, int _inventoryID) {
        TryUnequipItem(_id, _inventoryID, ref m_Data.equipment.armor);
        TryUnequipItem(_id, _inventoryID, ref m_Data.equipment.weapons);

        // redraw inventory if the window is open
        if (inventoryWindow) {
            inventoryWindow.Redraw();
        }

        // redraw equipment if the window is open
        if (equipmentWindow) {
            equipmentWindow.Redraw();
        }
    }
#endregion

#region Private Functions
    /// <summary>
    /// When player is created (ConnectWithData ^^^) we receive a json string array for each item
    /// This function builds the ItemData array, parsing ItemData entries into various children..
    /// ..such as ArmorItem, WeaponItem, etc..
    /// </summary>
    private void InitializeInventory() {
        m_Data.inventory = new List<IItem>();
        foreach(string _itemJson in m_Data.inventoryData) {
            IItem _item = ItemData.CreateItem(_itemJson);
            m_Data.inventory.Add(_item);
        }
    }

    private void InitializeEquipment() {
        m_Data.equipment = new PlayerEquipmentData();
        m_Data.equipment.armor = new List<ArmorItemData>();
        m_Data.equipment.weapons = new List<WeaponItemData>();

        if (m_Data.equipmentData == null) {
            InitializeTestEquipment();
            return;
        }

        foreach(string _itemJson in m_Data.equipmentData) {

            IItem _item = ItemData.CreateItem(_itemJson);

            switch (_item.def.itemType) {
                case ItemType.WEAPON: m_Data.equipment.weapons.Add((WeaponItemData)_item); break;
                case ItemType.ARMOR: m_Data.equipment.armor.Add((ArmorItemData)_item); break;
                default: break;
            }
        }
    }

    private void InitializeTestEquipment() {
        m_Data.equipment = new PlayerEquipmentData();
        m_Data.equipment.armor = new List<ArmorItemData>();
        m_Data.equipment.weapons = new List<WeaponItemData>();

        // add your weapons to test here
        // id is kind of irrelevant in test mode
        m_Data.equipment.weapons.Add(new WeaponItemData(123, GearType.R_HAND, "/db/icons/low-amp-phaser.png"));

        // access the weapon type and id
        Log("Weapon id: "+m_Data.equipment.weapons[0].def.id);
        Log("Weapon type: "+m_Data.equipment.weapons[0].slotType);

        // accessing this from the session...
        // session.player.data.equipment.weapons[0].def.id
        // session.player.data.equipment.weapons[0].weaponType
    }

    private void InitializeStats() {
        m_GearStats = new StatData();
        m_BuffStats = new StatData();
        m_TrickleStats = new StatData();
    }

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

    private void TryUnequipItem<T>(int _id, int _inventoryID, ref List<T> _arr) where T : IItem {
        for (int i = _arr.Count - 1; i >= 0; i--) {
            if (_arr[i].def.id == _id) {
                _arr[i].def.slotLoc = -1;
                _arr[i].def.slotID = _inventoryID;
                m_Data.inventory.Add(_arr[i]);
                //session.network.AddInventory(_arr[i].def);
                _arr.RemoveAt(i);
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
