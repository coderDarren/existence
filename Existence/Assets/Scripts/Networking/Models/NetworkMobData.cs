
public class NetworkMobData : NetworkModel {
    public string name;
    public int level;
    public int health;
    public int energy;
    public float attackSpeed;
    public float aggroRange;
    public bool inCombat;
    public NetworkVector3 pos;
    public NetworkVector3 rot;

    public NetworkMobData() {
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}