
public class NetworkPlayerHitInfo : NetworkModel {
    public string mobId;
    public string mobName; 
    public string playerName;
    public int dmg;
    public int health;
}

/// <summary>
/// Data for individual players
/// Each player will send this info up for..
/// ..other players to receive
///
/// Ideally the client sends up their NetworkPlayerData..
/// ..and receives NetworkInstanceData
/// </summary>
public class NetworkPlayerData : NetworkModel
{
    public int id;
    public string name;
    public string weaponName;
    public string specialName;
    public NetworkPlayerInput input;
    public NetworkVector3 pos;
    public NetworkVector3 rot;
    public int health;
    public int maxHealth;
    public int energy;
    public int maxEnergy;
    public int lvl;
    public int tix;
    public PlayerEquipmentData equipment;

    public NetworkPlayerData() {
        input = new NetworkPlayerInput();
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}


public class NetworkPlayerAnimation : NetworkModel {
    public float running;
    public float attackSpeed;
    public bool grounded;
    public bool attacking;
    public bool cycle;
    public bool special;   
}

public class NetworkPlayerEvent : NetworkModel {
    public string name;
    public NetworkPlayerEvent(string _n) {
        name = _n;
    }
}

public class NetworkPlayerAttackStart : NetworkPlayerEvent {
    public float attackSpeed;
    public string weaponName;
    public NetworkPlayerAttackStart(string _n, float _a, string _w) : base(_n) {
        name = _n;
        attackSpeed = _a;
        weaponName = _w;
    }
}

public class NetworkPlayerAttackStop : NetworkPlayerEvent {
    public NetworkPlayerAttackStop(string _n) : base(_n) {
        name = _n;
    }
}

public class NetworkPlayerUseSpecial : NetworkPlayerEvent {
    public string specialName;
    public NetworkPlayerUseSpecial(string _n, string _s) : base(_n) {
        name = _n;
        specialName = _s;
    }
}

public class NetworkPlayerHealthChange : NetworkPlayerEvent {
    public int health;
    public int maxHealth;
    public NetworkPlayerHealthChange(string _n, int _h, int _m) : base(_n) {
        name = _n;
        health = _h;
        maxHealth = _m;
    }
}

public class NetworkPlayerLvlChange : NetworkPlayerEvent {
    public int lvl;
    public NetworkPlayerLvlChange(string _n, int _l) : base(_n) {
        name = _n;
        lvl = _l;
    }
}

public class NetworkPlayerTransformChange : NetworkPlayerEvent {
    public NetworkVector3 pos;
    public NetworkVector3 rot;
    public NetworkPlayerTransformChange(string _n, NetworkVector3 _p, NetworkVector3 _r) : base(_n) {
        name = _n;
        pos = _p;
        rot = _r;
    }
}