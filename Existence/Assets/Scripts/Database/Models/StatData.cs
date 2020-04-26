using System.Collections;

[System.Serializable]
public class StatData : NetworkModel
{
    public int ID;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int fortitude;
    public int nanoPool;
    public int nanoResist;
    public int treatment;
    public int firstAid;
    public int oneHandEdged;
    public int twoHandEdged;
    public int pistol;
    public int shotgun;
    public int evades;
    public int crit;
    public int attackSpeed;
    public int hacking;
    public int engineering;
    public int programming;
    public int quantumMechanics;
    public int symbiotics;
    public int processing;
    public int runSpeed;
    public int melee;

    public static StatData Copy(StatData _in) {
        StatData _out = new StatData();
        _out.ID = _in.ID;
        _out.strength = _in.strength;
        _out.dexterity = _in.dexterity;
        _out.intelligence = _in.intelligence;
        _out.fortitude = _in.fortitude;
        _out.nanoPool = _in.nanoPool;
        _out.nanoResist = _in.nanoResist;
        _out.treatment = _in.treatment;
        _out.firstAid = _in.firstAid;
        _out.oneHandEdged = _in.oneHandEdged;
        _out.twoHandEdged = _in.twoHandEdged;
        _out.pistol = _in.pistol;
        _out.shotgun = _in.shotgun;
        _out.evades = _in.evades;
        _out.crit = _in.crit;
        _out.attackSpeed = _in.attackSpeed;
        _out.hacking = _in.hacking;
        _out.engineering = _in.engineering;
        _out.programming = _in.programming;
        _out.quantumMechanics = _in.quantumMechanics;
        _out.symbiotics = _in.symbiotics;
        _out.processing = _in.processing;
        _out.runSpeed = _in.runSpeed;
        _out.melee = _in.melee;
        return _out;
    }

    public static StatData FromHashtable(Hashtable _hash) {
        StatData _out = new StatData();
        _out.ID = (int)_hash["ID"];
        _out.strength = (int)_hash["strength"];
        _out.dexterity = (int)_hash["dexterity"];
        _out.intelligence = (int)_hash["intelligence"];
        _out.fortitude = (int)_hash["fortitude"];
        _out.nanoPool = (int)_hash["nanoPool"];
        _out.nanoResist = (int)_hash["nanoResist"];
        _out.treatment = (int)_hash["treatment"];
        _out.firstAid = (int)_hash["firstAid"];
        _out.oneHandEdged = (int)_hash["oneHandEdged"];
        _out.twoHandEdged = (int)_hash["twoHandEdged"];
        _out.pistol = (int)_hash["pistol"];
        _out.shotgun = (int)_hash["shotgun"];
        _out.evades = (int)_hash["evades"];
        _out.crit = (int)_hash["crit"];
        _out.attackSpeed = (int)_hash["attackSpeed"];
        _out.hacking = (int)_hash["hacking"];
        _out.engineering = (int)_hash["engineering"];
        _out.programming = (int)_hash["programming"];
        _out.quantumMechanics = (int)_hash["quantumMechanics"];
        _out.symbiotics = (int)_hash["symbiotics"];
        _out.processing = (int)_hash["processing"];
        _out.runSpeed = (int)_hash["runSpeed"];
        _out.melee = (int)_hash["melee"];
        return _out;
    }

    public static StatData TrickleFrom(StatData _in) {
        StatData _out = new StatData();
        StatData _str = StatData.StrengthTrickleRatios();
        StatData _dex = StatData.DexterityTrickleRatios();
        StatData _intel = StatData.IntelligenceTrickleRatios();
        _out.ID = _in.ID;

        // strength
        _out.fortitude = (int)(_in.strength * (_str.fortitude / (float)_str.strength));
        _out.oneHandEdged = (int)(_in.strength * (_str.oneHandEdged / (float)_str.strength));
        _out.twoHandEdged = (int)(_in.strength * (_str.twoHandEdged / (float)_str.strength));
        _out.melee = (int)(_in.strength * (_str.melee / (float)_str.strength));
        _out.pistol = (int)(_in.strength * (_str.pistol / (float)_str.strength));
        _out.shotgun = (int)(_in.strength * (_str.shotgun / (float)_str.strength));
        _out.crit = (int)(_in.strength * (_str.crit / (float)_str.strength));

        // dexterity
        _out.runSpeed = (int)(_in.dexterity * (_dex.runSpeed / (float)_dex.dexterity));
        _out.attackSpeed = (int)(_in.dexterity * (_dex.attackSpeed / (float)_dex.dexterity));
        _out.evades = (int)(_in.dexterity * (_dex.evades / (float)_dex.dexterity));

        // intel
        _out.nanoPool = (int)(_in.intelligence * (_intel.nanoPool / (float)_intel.intelligence));
        _out.nanoResist = (int)(_in.intelligence * (_intel.nanoResist / (float)_intel.intelligence));
        _out.treatment = (int)(_in.intelligence * (_intel.treatment / (float)_intel.intelligence));
        _out.firstAid = (int)(_in.intelligence * (_intel.firstAid / (float)_intel.intelligence));
        _out.hacking = (int)(_in.intelligence * (_intel.hacking / (float)_intel.intelligence));
        _out.engineering = (int)(_in.intelligence * (_intel.engineering / (float)_intel.intelligence));
        _out.programming = (int)(_in.intelligence * (_intel.programming / (float)_intel.intelligence));
        _out.quantumMechanics = (int)(_in.intelligence * (_intel.quantumMechanics / (float)_intel.intelligence));
        _out.symbiotics = (int)(_in.intelligence * (_intel.symbiotics / (float)_intel.intelligence));
        _out.processing = (int)(_in.intelligence * (_intel.processing / (float)_intel.intelligence));
        return _out;
    }

    public static StatData StrengthTrickleRatios() {
        StatData _out = new StatData();
        _out.strength = 12;
        _out.fortitude = 3; // 0.25
        _out.oneHandEdged = 3; // 0.167
        _out.twoHandEdged = 3; // 0.25
        _out.pistol = 2; // 0.167
        _out.shotgun = 2; // 0.167
        _out.crit = 2; // 0.167
        _out.melee = 3; // 0.167
        return _out;
    }

    public static StatData DexterityTrickleRatios() {
        StatData _out = new StatData();
        _out.dexterity = 12;
        _out.evades = 3; // 0.25
        _out.attackSpeed = 3; // 0.25
        _out.runSpeed = 3; // 0.25
        return _out;
    }

    public static StatData IntelligenceTrickleRatios() {
        StatData _out = new StatData();
        _out.intelligence = 12;
        _out.nanoResist = 2; // 0.167
        _out.treatment = 2; // 0.167
        _out.firstAid = 2; // 0.167
        _out.hacking = 3; // 0.25
        _out.engineering = 3; // 0.25
        _out.programming = 3; // 0.25
        _out.quantumMechanics = 3; // 0.25
        _out.symbiotics = 3; // 0.25
        _out.processing = 3; // 0.25
        return _out;
    }

    public StatData Combine(StatData _in) {
        StatData _out = StatData.Copy(this);
        _out.ID = _in.ID;
        _out.strength += _in.strength;
        _out.dexterity += _in.dexterity;
        _out.intelligence += _in.intelligence;
        _out.fortitude += _in.fortitude;
        _out.nanoPool += _in.nanoPool;
        _out.nanoResist += _in.nanoResist;
        _out.treatment += _in.treatment;
        _out.firstAid += _in.firstAid;
        _out.oneHandEdged += _in.oneHandEdged;
        _out.twoHandEdged += _in.twoHandEdged;
        _out.pistol += _in.pistol;
        _out.shotgun += _in.shotgun;
        _out.evades += _in.evades;
        _out.crit += _in.crit;
        _out.attackSpeed += _in.attackSpeed;
        _out.hacking += _in.hacking;
        _out.engineering += _in.engineering;
        _out.programming += _in.programming;
        _out.quantumMechanics += _in.quantumMechanics;
        _out.symbiotics += _in.symbiotics;
        _out.processing += _in.processing;
        _out.runSpeed += _in.runSpeed;
        _out.melee += _in.melee;
        return _out;
    }

    public Hashtable ToHashtable() {
        Hashtable _out = new Hashtable();
        _out["ID"] = this.ID;
        _out["strength"] = this.strength;
        _out["dexterity"] = this.dexterity;
        _out["intelligence"] = this.intelligence;
        _out["fortitude"] = this.fortitude;
        _out["nanoPool"] = this.nanoPool;
        _out["nanoResist"] = this.nanoResist;
        _out["treatment"] = this.treatment;
        _out["firstAid"] = this.firstAid;
        _out["oneHandEdged"] = this.oneHandEdged;
        _out["twoHandEdged"] = this.twoHandEdged;
        _out["pistol"] = this.pistol;
        _out["shotgun"] = this.shotgun;
        _out["evades"] = this.evades;
        _out["crit"] = this.crit;
        _out["attackSpeed"] = this.attackSpeed;
        _out["hacking"] = this.hacking;
        _out["engineering"] = this.engineering;
        _out["programming"] = this.programming;
        _out["quantumMechanics"] = this.quantumMechanics;
        _out["symbiotics"] = this.symbiotics;
        _out["processing"] = this.processing;
        _out["runSpeed"] = this.runSpeed;
        _out["melee"] = this.melee;
        return _out;
    }
}
