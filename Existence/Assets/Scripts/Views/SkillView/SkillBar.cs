
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

#region Private Functions
    private void OnEnable() {
        // initialize the bar
        AddBar(0);
    }

    private void UpdateView(string _label, int _val, int _max) {
        skillLabel.text = _label;
        valLabel.text = _val.ToString();
        bar.fillAmount = _val / (float)_max;
    }

    private bool AddBar(int _diff) {
        if (!session) return false;

        switch (stat) {
            case StatType.STRENGTH: UpdateView("Strength", session.playerData.stats.strength + _diff, 100); break;
            case StatType.DEXTERITY: UpdateView("Dexterity", session.playerData.stats.dexterity + _diff, 100); break;
            case StatType.INTELLIGENCE: UpdateView("Intelligence", session.playerData.stats.intelligence + _diff, 100); break;
            case StatType.FORTITUDE: UpdateView("Fortitude", session.playerData.stats.fortitude + _diff, 100); break;
            case StatType.NANO_POOL: UpdateView("Nano Pool", session.playerData.stats.nanoPool + _diff, 100); break;
            case StatType.NANO_RESIST: UpdateView("Nano Resist", session.playerData.stats.nanoResist + _diff, 100); break;
            case StatType.TREATMENT: UpdateView("Treatment", session.playerData.stats.treatment + _diff, 100); break;
            case StatType.FIRST_AID: UpdateView("First Aid", session.playerData.stats.firstAid + _diff, 100); break;
            case StatType.ONE_HAND_EDGED: UpdateView("1 Hand Edged", session.playerData.stats.oneHandEdged + _diff, 100); break;
            case StatType.TWO_HAND_EDGED: UpdateView("2 Hand Edged", session.playerData.stats.twoHandEdged + _diff, 100); break;
            case StatType.PISTOL: UpdateView("Pistol", session.playerData.stats.pistol + _diff, 100); break;
            case StatType.SHOTGUN: UpdateView("Shotgun", session.playerData.stats.shotgun + _diff, 100); break;
            case StatType.EVADES: UpdateView("Evades", session.playerData.stats.evades + _diff, 100); break;
            case StatType.CRIT: UpdateView("Crit", session.playerData.stats.crit + _diff, 100); break;
            case StatType.ATTACK_SPEED: UpdateView("Attack Speed", session.playerData.stats.attackSpeed + _diff, 100); break;
            case StatType.HACKING: UpdateView("Hacking", session.playerData.stats.hacking + _diff, 100); break;
            case StatType.ENGINEERING: UpdateView("Engineering", session.playerData.stats.engineering + _diff, 100); break;
            case StatType.PROGRAMMING: UpdateView("Programming", session.playerData.stats.programming + _diff, 100); break;
            case StatType.QUANTUM_MECHANICS: UpdateView("Quantum Mechanics", session.playerData.stats.quantumMechanics + _diff, 100); break;
            case StatType.SYMBIOTICS: UpdateView("Symbiotics", session.playerData.stats.symbiotics + _diff, 100); break;
            case StatType.PROCESSING: UpdateView("Processing", session.playerData.stats.processing + _diff, 100); break;
            case StatType.RUN_SPEED: UpdateView("Run Speed", session.playerData.stats.runSpeed + _diff, 100); break;
            case StatType.MELEE: UpdateView("Melee", session.playerData.stats.melee + _diff, 100); break;
        }

        return bar.fillAmount != 1;
    }
#endregion

#region Public Functions
    public bool Add(int _diff) {
        return AddBar(_diff);
    }
#endregion
}
