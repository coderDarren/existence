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

    private YieldInstruction m_InitialIncrement;
    private YieldInstruction m_SpeedIncrement;

#region Unity Functions
    private void OnEnable() {
        m_InitialIncrement = new WaitForSeconds(1);
        m_SpeedIncrement = new WaitForSeconds(0.1f);
    }

    private void OnDisable() {
        StopCoroutine("Press");
    }
#endregion

#region Private Functions
    private bool Add() {
        int _diff = modType == ModType.ADD ? 1 : -1;
        return skillBar.Add(_diff);
    }

    private IEnumerator Press() {
        Add();
        yield return m_InitialIncrement;
        Add();
        yield return m_InitialIncrement;
        while (Add()) {
            yield return m_SpeedIncrement;
        }
    }
#endregion

#region Interface Functions
    public void OnPointerDown(PointerEventData _ped) {
        StartCoroutine("Press");
    }

    public void OnPointerUp(PointerEventData _ped) {
        StopCoroutine("Press");
    }
#endregion
}
