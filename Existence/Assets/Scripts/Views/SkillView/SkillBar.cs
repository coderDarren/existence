﻿using System.Collections;
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
    private int m_SkillMax=50000;

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
            case StatType.STRENGTH: 
                _skills.strength = Mathf.Clamp(_skills.strength += _diff, 0, m_SkillMax);
                UpdateView("Strength", _skills.strength, m_SkillMax); 
                break;
            case StatType.DEXTERITY: 
                _skills.dexterity = Mathf.Clamp(_skills.dexterity += _diff, 0, m_SkillMax);
                UpdateView("Dexterity", _skills.dexterity += _diff, m_SkillMax); 
                break;
            case StatType.INTELLIGENCE: 
                _skills.intelligence = Mathf.Clamp(_skills.intelligence += _diff, 0, m_SkillMax);
                UpdateView("Intelligence", _skills.intelligence += _diff, m_SkillMax); 
                break;
            case StatType.FORTITUDE: 
                _skills.fortitude = Mathf.Clamp(_skills.fortitude += _diff, 0, m_SkillMax);
                UpdateView("Fortitude", _skills.fortitude += _diff, m_SkillMax); 
                break;
            case StatType.NANO_POOL: 
                _skills.nanoPool = Mathf.Clamp(_skills.nanoPool += _diff, 0, m_SkillMax);
                UpdateView("Nano Pool", _skills.nanoPool += _diff, m_SkillMax); 
                break;
            case StatType.NANO_RESIST: 
                _skills.nanoResist = Mathf.Clamp(_skills.nanoResist += _diff, 0, m_SkillMax);
                UpdateView("Nano Resist", _skills.nanoResist += _diff, m_SkillMax); 
                break;
            case StatType.TREATMENT: 
                _skills.treatment = Mathf.Clamp(_skills.treatment += _diff, 0, m_SkillMax);
                UpdateView("Treatment", _skills.treatment += _diff, m_SkillMax); 
                break;
            case StatType.FIRST_AID: 
                _skills.firstAid = Mathf.Clamp(_skills.firstAid += _diff, 0, m_SkillMax);
                UpdateView("First Aid", _skills.firstAid += _diff, m_SkillMax); 
                break;
            case StatType.ONE_HAND_EDGED: 
                _skills.oneHandEdged = Mathf.Clamp(_skills.oneHandEdged += _diff, 0, m_SkillMax);
                UpdateView("1 Hand Edged", _skills.oneHandEdged += _diff, m_SkillMax); 
                break;
            case StatType.TWO_HAND_EDGED: 
                _skills.twoHandEdged = Mathf.Clamp(_skills.twoHandEdged += _diff, 0, m_SkillMax);
                UpdateView("2 Hand Edged", _skills.twoHandEdged += _diff, m_SkillMax); 
                break;
            case StatType.PISTOL: 
                _skills.pistol = Mathf.Clamp(_skills.pistol += _diff, 0, m_SkillMax);
                UpdateView("Pistol", _skills.pistol += _diff, m_SkillMax); 
                break;
            case StatType.SHOTGUN: 
                _skills.shotgun = Mathf.Clamp(_skills.shotgun += _diff, 0, m_SkillMax);
                UpdateView("Shotgun", _skills.shotgun += _diff, m_SkillMax); 
                break;
            case StatType.EVADES: 
                _skills.evades = Mathf.Clamp(_skills.evades += _diff, 0, m_SkillMax);
                UpdateView("Evades", _skills.evades += _diff, m_SkillMax); 
                break;
            case StatType.CRIT: 
                _skills.crit = Mathf.Clamp(_skills.crit += _diff, 0, m_SkillMax);
                UpdateView("Crit", _skills.crit += _diff, m_SkillMax); 
                break;
            case StatType.ATTACK_SPEED: 
                _skills.attackSpeed = Mathf.Clamp(_skills.attackSpeed += _diff, 0, m_SkillMax);
                UpdateView("Attack Speed", _skills.attackSpeed += _diff, m_SkillMax); 
                break;
            case StatType.HACKING: 
                _skills.hacking = Mathf.Clamp(_skills.hacking += _diff, 0, m_SkillMax);
                UpdateView("Hacking", _skills.hacking += _diff, m_SkillMax); 
                break;
            case StatType.ENGINEERING: 
                _skills.engineering = Mathf.Clamp(_skills.engineering += _diff, 0, m_SkillMax);
                UpdateView("Engineering", _skills.engineering += _diff, m_SkillMax); 
                break;
            case StatType.PROGRAMMING: 
                _skills.programming = Mathf.Clamp(_skills.programming += _diff, 0, m_SkillMax);
                UpdateView("Programming", _skills.programming += _diff, m_SkillMax); 
                break;
            case StatType.QUANTUM_MECHANICS: 
                _skills.quantumMechanics = Mathf.Clamp(_skills.quantumMechanics += _diff, 0, m_SkillMax);
                UpdateView("Quantum Mechanics", _skills.quantumMechanics += _diff, m_SkillMax); 
                break;
            case StatType.SYMBIOTICS: 
                _skills.symbiotics = Mathf.Clamp(_skills.symbiotics += _diff, 0, m_SkillMax);
                UpdateView("Symbiotics", _skills.symbiotics += _diff, m_SkillMax); 
                break;
            case StatType.PROCESSING: 
                _skills.processing = Mathf.Clamp(_skills.processing += _diff, 0, m_SkillMax);
                UpdateView("Processing", _skills.processing += _diff, m_SkillMax); 
                break;
            case StatType.RUN_SPEED: 
                _skills.runSpeed = Mathf.Clamp(_skills.runSpeed += _diff, -m_SkillMax, m_SkillMax);
                UpdateView("Run Speed", _skills.runSpeed += _diff, m_SkillMax); 
                break;
            case StatType.MELEE: 
                _skills.melee = Mathf.Clamp(_skills.melee += _diff, 0, m_SkillMax);
                UpdateView("Melee", _skills.melee += _diff, m_SkillMax); 
                break;
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
