using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : GameSystem
{
    public SkillsPage page;
    public StatType stat;
    public Image bar;
    public Text skillLabel;
    public Text valLabel;

    private Session m_Session;

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
        AddBar(session.playerData.stats, 0);
    }
#endregion

#region Private Functions
    private void UpdateView(string _label, int _val, int _max) {
        skillLabel.text = _label;
        valLabel.text = _val.ToString();
        bar.fillAmount = _val / (float)_max;
    }

    /*
     * Input skills will differ depending on whether we are initializing or updating
     * We use a copy of player data to update (to deal with saving)
     * We use player data to initialize (to make sure unsaved data isnt used)
     */
    private bool AddBar(StatData _skills, int _diff) {
        if (!session) return false;

        switch (stat) {
            case StatType.STRENGTH: UpdateView("Strength", _skills.strength += _diff, 100); break;
            case StatType.DEXTERITY: UpdateView("Dexterity", _skills.dexterity += _diff, 100); break;
            case StatType.INTELLIGENCE: UpdateView("Intelligence", _skills.intelligence += _diff, 100); break;
            case StatType.FORTITUDE: UpdateView("Fortitude", _skills.fortitude += _diff, 100); break;
            case StatType.NANO_POOL: UpdateView("Nano Pool", _skills.nanoPool += _diff, 100); break;
            case StatType.NANO_RESIST: UpdateView("Nano Resist", _skills.nanoResist += _diff, 100); break;
            case StatType.TREATMENT: UpdateView("Treatment", _skills.treatment += _diff, 100); break;
            case StatType.FIRST_AID: UpdateView("First Aid", _skills.firstAid += _diff, 100); break;
            case StatType.ONE_HAND_EDGED: UpdateView("1 Hand Edged", _skills.oneHandEdged += _diff, 100); break;
            case StatType.TWO_HAND_EDGED: UpdateView("2 Hand Edged", _skills.twoHandEdged += _diff, 100); break;
            case StatType.PISTOL: UpdateView("Pistol", _skills.pistol += _diff, 100); break;
            case StatType.SHOTGUN: UpdateView("Shotgun", _skills.shotgun += _diff, 100); break;
            case StatType.EVADES: UpdateView("Evades", _skills.evades += _diff, 100); break;
            case StatType.CRIT: UpdateView("Crit", _skills.crit += _diff, 100); break;
            case StatType.ATTACK_SPEED: UpdateView("Attack Speed", _skills.attackSpeed += _diff, 100); break;
            case StatType.HACKING: UpdateView("Hacking", _skills.hacking += _diff, 100); break;
            case StatType.ENGINEERING: UpdateView("Engineering", _skills.engineering += _diff, 100); break;
            case StatType.PROGRAMMING: UpdateView("Programming", _skills.programming += _diff, 100); break;
            case StatType.QUANTUM_MECHANICS: UpdateView("Quantum Mechanics", _skills.quantumMechanics += _diff, 100); break;
            case StatType.SYMBIOTICS: UpdateView("Symbiotics", _skills.symbiotics += _diff, 100); break;
            case StatType.PROCESSING: UpdateView("Processing", _skills.processing += _diff, 100); break;
            case StatType.RUN_SPEED: UpdateView("Run Speed", _skills.runSpeed += _diff, 100); break;
            case StatType.MELEE: UpdateView("Melee", _skills.melee += _diff, 100); break;
        }

        return bar.fillAmount != 1;
    }
#endregion

#region Public Functions
    public bool Add(int _diff) {
        return AddBar(page.skills, _diff);
    }
#endregion
}
