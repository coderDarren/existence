using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using ProScripts;

public class SkillBarButton : ProButton
{
    
    public enum ModType {
        ADD,
        SUB
    }

    public SkillBar skillBar;
    public ModType modType;

    private YieldInstruction m_BaseIncrement;
    private YieldInstruction m_SpeedIncrement;
    private bool m_ShiftIsDown;

#region Unity Functions
    private void Update() {
        m_ShiftIsDown = Input.GetKey(KeyCode.LeftShift);
    }
#endregion

#region Private Functions
    private bool Add(int _amount=1) {
        int _diff = modType == ModType.ADD ? 1*_amount : -1*_amount;
        return skillBar.Add(_diff);
    }

    private IEnumerator Press() {
        int _factor = 1;
        Add();
        yield return m_BaseIncrement;
        Add();
        yield return m_BaseIncrement;
        while (Add(_factor)) {
            if (m_ShiftIsDown) {
                _factor = 10;
            } else {
                _factor = 1;
            }
            yield return m_SpeedIncrement;
        }
    }
#endregion

#region Public Functions
    public void StartPress() {
        StopCoroutine("Press");
        StartCoroutine("Press");
    }

    public void StopPress() {
        StopCoroutine("Press");
    }
#endregion

#region Override Functions
    public override void Init() {
        base.Init();
        m_BaseIncrement = new WaitForSeconds(0.5f);
        m_SpeedIncrement = new WaitForSeconds(0.015f);
    }

    public override void Dispose() {
        base.Dispose();
        StopPress();
    }
#endregion
}
