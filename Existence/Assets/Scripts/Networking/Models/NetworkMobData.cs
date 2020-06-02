
public class NetworkMobHitInfo : NetworkModel {
    public string id;
    public int dmg;
    public NetworkMobHitInfo(string _id, int _dmg) {
        id = _id;
        dmg = _dmg;
    }
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