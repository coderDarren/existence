
public class NetworkMobHitInfo : NetworkModel {
    public string id;
    public string mobName;
    public string playerName;
    public int dmg;
    public bool crit;
    public NetworkMobHitInfo(string _id, string _mobName, int _dmg, bool _crit) {
        id = _id;
        mobName = _mobName;
        dmg = _dmg;
        crit = _crit;
    }
}

public class NetworkMobAttackData : NetworkModel {
    public string id;
    public string mobName;
    public string playerName;
}

public class NetworkMobDeathData : NetworkModel {
    public string id;
    public string name;
    public string[] lootRights; // array of player names who can loot
    public NetworkMobXpAllottment[] xpAllottment;
    public NetworkLootPreviewData[] lootPreview; // array of item previews
}

public class NetworkMobXpAllottment : NetworkModel {
    public string playerName;
    public int xp;
}

public class NetworkMobData : NetworkModel {
    public string id;
    public string name;
    public int level;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public float attackSpeed;
    public float aggroRange;
    public bool inAttackRange;
    public bool inCombat;
    public bool dead;
    public NetworkLootPreviewData[] lootPreview;
    public NetworkVector3 pos;
    public NetworkVector3 rot;

    public NetworkMobData() {
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}