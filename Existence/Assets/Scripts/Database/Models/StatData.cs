
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
}
