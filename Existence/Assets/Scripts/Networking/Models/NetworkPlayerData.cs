
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
    public NetworkTransform transform;
    public int health;
    public int maxHealth;
    public int energy;
    public int maxEnergy;
    public int lvl;
    public int tix;
    public PlayerEquipmentData equipment;

    public NetworkPlayerData() {
        input = new NetworkPlayerInput();
        transform = new NetworkTransform();
        equipment = new PlayerEquipmentData();
    }

    public void UpdatePos(UnityEngine.Vector3 _pos) {
        transform.id = name;
        transform.pos.x = _pos.x;
        transform.pos.y = _pos.y;
        transform.pos.z = _pos.z;
    }

    public void UpdateRot(UnityEngine.Vector3 _rot) {
        transform.id = name;
        transform.rot.x = _rot.x;
        transform.rot.y = _rot.y;
        transform.rot.z = _rot.z;
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
    public string id;
    public NetworkPlayerEvent(string _n) {
        id = _n;
    }
}

public class NetworkPlayerAttackStart : NetworkPlayerEvent {
    public float attackSpeed;
    public string weaponName;
    public NetworkPlayerAttackStart(string _n, float _a, string _w) : base(_n) {
        id = _n;
        attackSpeed = _a;
        weaponName = _w;
    }
}

public class NetworkPlayerAttackStop : NetworkPlayerEvent {
    public NetworkPlayerAttackStop(string _n) : base(_n) {
        id = _n;
    }
}

public class NetworkPlayerUseSpecial : NetworkPlayerEvent {
    public string specialName;
    public NetworkPlayerUseSpecial(string _n, string _s) : base(_n) {
        id = _n;
        specialName = _s;
    }
}

public class NetworkPlayerHealth : NetworkPlayerEvent {
    public int health;
    public int maxHealth;
    public NetworkPlayerHealth(string _n, int _h, int _m) : base(_n) {
        id = _n;
        health = _h;
        maxHealth = _m;
    }
}

public class NetworkPlayerLvl : NetworkPlayerEvent {
    public int lvl;
    public NetworkPlayerLvl(string _n, int _l) : base(_n) {
        id = _n;
        lvl = _l;
    }
}

public class NetworkTransform : NetworkPlayerEvent {
    public NetworkVector3 pos;
    public NetworkVector3 rot;
    public NetworkTransform() : base("") {
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}

public class NetworkAnim : NetworkPlayerEvent {
    public string anim;
    public NetworkAnim(string _id, string _anim) : base(_id) {
        anim = _anim;
    }
}

public class NetworkAnimFloat : NetworkAnim {
    public float val;
    public NetworkAnimFloat(string _anim) : base("", _anim) {
        val = 0;
    }
}

public class NetworkAnimBool : NetworkAnim {
    public bool val;
    public NetworkAnimBool(string _anim) : base("", _anim) {
        val = false;
    }
}