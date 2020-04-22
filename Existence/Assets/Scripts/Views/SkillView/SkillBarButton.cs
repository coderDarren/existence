using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBarButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
    private void OnEnable() {
        m_BaseIncrement = new WaitForSeconds(0.5f);
        m_SpeedIncrement = new WaitForSeconds(0.015f);
    }

    private void OnDisable() {
        StopCoroutine("Press");
    }

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

#region Interface Functions
    public void OnPointerDown(PointerEventData _ped) {
        StopCoroutine("Press");
        StartCoroutine("Press");
    }

    public void OnPointerUp(PointerEventData _ped) {
        StopCoroutine("Press");
    }
#endregion
}
