
public class NetworkPlayerHitInfo : NetworkModel {
    public string mobId;
    public string mobName; 
    public string playerName;
    public int dmg;
    public int health;
}

/*
 *
 */
public class NetworkPlayerData : NetworkModel
{
    public int id;
    public string name;
    public int health;
    public int maxHealth;
    public int lvl;
    public int tix;
    public NetworkPlayerAnimation anim;
    public NetworkTransform transform;
    public PlayerEquipmentData equipment;

    public NetworkPlayerData() {
        transform = new NetworkTransform();
        equipment = new PlayerEquipmentData();
        anim = new NetworkPlayerAnimation();
    }

    /*
     * Each of the updaters below need to ensure..
     * ..id is set to the player name so the receiver..
     * ..of the data knows who to direct information to
     */

#region Transform Updaters
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
#endregion

#region Animation Updaters
    public void UpdateRunning(float _val) {
        anim.running.id = name;
        anim.running.val = _val;
    }

    public void UpdateGrounded(bool _val) {
        anim.grounded.id = name;
        anim.grounded.val = _val;
    }

    public void UpdateCycle(bool _val) {
        anim.cycle.id = name;
        anim.cycle.val = _val;
    }

    public void UpdateAttacking(string _weapon, bool _val) {
        anim.attacking.id = name;
        anim.attacking.anim = _weapon;
        anim.attacking.val = _val;
    }

    public void UpdateSpecial(string _special, bool _val) {
        anim.special.id = name;
        anim.special.anim = _special;
        anim.special.val = _val;
    }
#endregion
}

public class NetworkPlayerAnimation : NetworkModel {
    public NetworkAnimFloat running;
    public NetworkAnimBool grounded;
    public NetworkAnimBool attacking;
    public NetworkAnimBool cycle;
    public NetworkAnimBool special;
    public NetworkPlayerAnimation() {
        running = new NetworkAnimFloat("running");
        grounded = new NetworkAnimBool("grounded");
        attacking = new NetworkAnimBool("");
        cycle = new NetworkAnimBool("cycle");
        special = new NetworkAnimBool("");;
    }
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