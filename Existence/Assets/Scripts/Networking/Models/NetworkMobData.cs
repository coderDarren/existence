
public class NetworkMobHitInfo : NetworkModel {
    public string id;
    public string mobName;
    public string playerName;
    public int dmg;
    public NetworkMobHitInfo(string _id, string _mobName, int _dmg) {
        id = _id;
        mobName = _mobName;
        dmg = _dmg;
    }
}

public class NetworkMobAttackData : NetworkModel {
    public string id;
    public string mobName;
    public string playerName;
}

public class NetworkMobDeathData : NetworkModel {
    public string id;
    public string[] lootRights; // array of player names who can loot
    public NetworkMobXpAllottment[] xpAllottment;
    public int[] loot; // array of item ids
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
    public NetworkVector3 pos;
    public NetworkVector3 rot;

    public NetworkMobData() {
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}