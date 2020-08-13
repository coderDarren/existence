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
    public int hot; // heal-over-time

    /*
     * Using this constructor, we can initialize some id-less StatData..
     * ..which may be useful when determining skill point maximums based on level
     */
    public StatData(int _scalar) {
        strength = _scalar;
        dexterity = _scalar;
        intelligence = _scalar;
        fortitude = _scalar;
        nanoPool = _scalar;
        nanoResist = _scalar;
        treatment = _scalar;
        firstAid = _scalar;
        oneHandEdged = _scalar;
        twoHandEdged = _scalar;
        pistol = _scalar;
        shotgun = _scalar;
        evades = _scalar;
        crit = _scalar;
        attackSpeed = _scalar;
        hacking = _scalar;
        engineering = _scalar;
        programming = _scalar;
        quantumMechanics = _scalar;
        symbiotics = _scalar;
        processing = _scalar;
        runSpeed = _scalar;
        melee = _scalar;
        hot = _scalar;
    }

    public StatData() {}

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
        _out.hot = _in.hot;
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
        _out.hot = (int)_hash["hot"];
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
        return OperateStats(_in, "+");
    }

    public StatData Reduce(StatData _in) {
        return OperateStats(_in, "-");
    }

    private StatData OperateStats(StatData _in, string _op) {
        StatData _out = StatData.Copy(this);
        _out.ID = _in.ID;
        _out.strength = OperateValues(_out.strength, _in.strength, _op);
        _out.dexterity = OperateValues(_out.dexterity, _in.dexterity, _op);
        _out.intelligence = OperateValues(_out.intelligence, _in.intelligence, _op);
        _out.fortitude = OperateValues(_out.fortitude, _in.fortitude, _op);
        _out.nanoPool = OperateValues(_out.nanoPool, _in.nanoPool, _op);
        _out.nanoResist = OperateValues(_out.nanoResist, _in.nanoResist, _op);
        _out.treatment = OperateValues(_out.treatment, _in.treatment, _op);
        _out.firstAid = OperateValues(_out.firstAid, _in.firstAid, _op);
        _out.oneHandEdged = OperateValues(_out.oneHandEdged, _in.oneHandEdged, _op);
        _out.twoHandEdged = OperateValues(_out.twoHandEdged, _in.twoHandEdged, _op);
        _out.pistol = OperateValues(_out.pistol, _in.pistol, _op);
        _out.shotgun = OperateValues(_out.shotgun, _in.shotgun, _op);
        _out.evades = OperateValues(_out.evades, _in.evades, _op);
        _out.crit = OperateValues(_out.crit, _in.crit, _op);
        _out.attackSpeed = OperateValues(_out.attackSpeed, _in.attackSpeed, _op);
        _out.hacking = OperateValues(_out.hacking, _in.hacking, _op);
        _out.engineering = OperateValues(_out.engineering, _in.engineering, _op);
        _out.programming = OperateValues(_out.programming, _in.programming, _op);
        _out.quantumMechanics = OperateValues(_out.quantumMechanics, _in.quantumMechanics, _op);
        _out.symbiotics = OperateValues(_out.symbiotics, _in.symbiotics, _op);
        _out.processing = OperateValues(_out.processing, _in.processing, _op);
        _out.runSpeed = OperateValues(_out.runSpeed, _in.runSpeed, _op);
        _out.melee = OperateValues(_out.melee, _in.melee, _op);
        _out.hot = OperateValues(_out.hot, _in.hot, _op);
        return _out;
    }

    private int OperateValues(int _val1, int _val2, string _op) {
        switch (_op) {
            case "+": return _val1 + _val2;
            case "-": return _val1 - _val2;
            case "*": return _val1 * _val2;
            case "/": return _val1 / _val2;
            default: return 0;
        }
    }

    /*
     * Compare is a good function to use to see if requirements are met for..
     * ..gear, buffs, etc..
     *
     * Returns 1 if requirements are met, -1 if requirements are not met
     *
     */
    public int Compare(StatData _other) {
        if (this.strength < _other.strength) return -1;
        else if (this.dexterity < _other.dexterity) return -1;
        else if (this.intelligence < _other.intelligence) return -1;
        else if (this.fortitude < _other.fortitude) return -1;
        else if (this.nanoPool < _other.nanoPool) return -1;
        else if (this.nanoResist < _other.nanoResist) return -1;
        else if (this.treatment < _other.treatment) return -1;
        else if (this.firstAid < _other.firstAid) return -1;
        else if (this.oneHandEdged < _other.oneHandEdged) return -1;
        else if (this.twoHandEdged < _other.twoHandEdged) return -1;
        else if (this.pistol < _other.pistol) return -1;
        else if (this.shotgun < _other.shotgun) return -1;
        else if (this.evades < _other.evades) return -1;
        else if (this.crit < _other.crit) return -1;
        else if (this.attackSpeed < _other.attackSpeed) return -1;
        else if (this.hacking < _other.hacking) return -1;
        else if (this.engineering < _other.engineering) return -1;
        else if (this.programming < _other.programming) return -1;
        else if (this.quantumMechanics < _other.quantumMechanics) return -1;
        else if (this.symbiotics < _other.symbiotics) return -1;
        else if (this.processing < _other.processing) return -1;
        else if (this.runSpeed < _other.runSpeed) return -1;
        else if (this.melee < _other.melee) return -1;
        else if (this.hot < _other.hot) return -1;
        else return 1;
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
        _out["hot"] = this.hot;
        return _out;
    }
}
