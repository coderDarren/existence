using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : GameSystem
{
    public SkillsPage page;
    public SkillSection section;
    public StatType stat;
    public Image bar;
    public Text skillLabel;
    public Text valLabel;

    private Session m_Session;
    private SkillSection m_Section;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Unity Functions
    private void OnEnable() {
        if (!session) return;
        AddBar(0);
    }

    private void OnDisable() {
        
    }
#endregion

#region Private Functions
    private void UpdateView(string _label, string _hash, int _added) {
        int _curr = (int)page.stats[_hash];
        int _max = (int)page.statMaximums.ToHashtable()[_hash];

        // don't allow reductions beyond the start of the allocation session
        if (_curr + _added < (int)session.player.data.stats.ToHashtable()[_hash]) return;
        // don't allow allocations beyond the maximum
        if (_curr + _added > _max) return;

        page.stats[(_hash)] = _curr + _added;

        StatData _aggregated = StatData.FromHashtable(page.stats).Combine(StatData.FromHashtable(page.otherStats));
        int _totalAggregate = (int)StatData.TrickleFrom(_aggregated).Combine(_aggregated).ToHashtable()[_hash]; // stats (gear, buffs, base, trickle)
        int _totalBuffed = (int)_aggregated.ToHashtable()[_hash]; // stats (gear, buffs, base)
        int _totalBase = (int)page.stats[_hash]; // stats (base)

        skillLabel.text = _label;

        // color the value green if the stat is otherwise buffed
        if (_totalBuffed > _totalBase) {
            valLabel.text = "<color=#0f0>"+_totalAggregate+"</color>";
        } else {
            valLabel.text = _totalAggregate.ToString();
        }

        // aggregate stats should not fill the stat bar..
        // ..only base stats should fill the stat bar.
        // Otherwise, buffs would prevent you from being able to level skills up
        bar.fillAmount = _totalBase / (float)_max;

        page.statPoints -= _added;
    }

    /*
     * Input skills will differ depending on whether we are initializing or updating
     * We use a copy of player data to update (to deal with saving)
     * We use player data to initialize (to make sure unsaved data isnt used)
     */
    private bool AddBar(int _diff) {
        if (!session) return false;

        switch (stat) {
            case StatType.STRENGTH: UpdateView("Strength", "strength", _diff); break;
            case StatType.DEXTERITY: UpdateView("Dexterity", "dexterity", _diff); break;
            case StatType.INTELLIGENCE: UpdateView("Intelligence", "intelligence", _diff); break;
            case StatType.FORTITUDE: UpdateView("Fortitude", "fortitude", _diff); break;
            case StatType.NANO_POOL: UpdateView("Nano Pool", "nanoPool", _diff); break;
            case StatType.NANO_RESIST: UpdateView("Nano Resist", "nanoResist", _diff); break;
            case StatType.TREATMENT: UpdateView("Treatment", "treatment", _diff); break;
            case StatType.FIRST_AID: UpdateView("First Aid", "firstAid", _diff); break;
            case StatType.ONE_HAND_EDGED: UpdateView("1 Hand Edged", "oneHandEdged", _diff); break;
            case StatType.TWO_HAND_EDGED: UpdateView("2 Hand Edged", "twoHandEdged", _diff); break;
            case StatType.PISTOL: UpdateView("Pistol", "pistol", _diff); break;
            case StatType.SHOTGUN: UpdateView("Shotgun", "shotgun", _diff); break;
            case StatType.EVADES: UpdateView("Evades", "evades", _diff); break;
            case StatType.CRIT: UpdateView("Critical Hit", "crit", _diff); break;
            case StatType.ATTACK_SPEED: UpdateView("Attack Speed", "attackSpeed", _diff); break;
            case StatType.HACKING: UpdateView("Hacking", "hacking", _diff); break;
            case StatType.ENGINEERING: UpdateView("Engineering", "engineering", _diff); break;
            case StatType.PROGRAMMING: UpdateView("Programming", "programming", _diff); break;
            case StatType.QUANTUM_MECHANICS: UpdateView("Quantum Mechanics", "quantumMechanics", _diff); break;
            case StatType.SYMBIOTICS: UpdateView("Symbiotics", "symbiotics", _diff); break;
            case StatType.PROCESSING: UpdateView("Processing", "processing", _diff); break;
            case StatType.RUN_SPEED: UpdateView("Run Speed", "runSpeed", _diff); break;
            case StatType.MELEE: UpdateView("Melee", "melee", _diff); break;
        }

        return bar.fillAmount != 1;
    }
#endregion

#region Public Functions
    public bool Add(int _diff) {
        if (page.statPoints - _diff < 0) return false;
        return AddBar(_diff);
    }
#endregion
}
