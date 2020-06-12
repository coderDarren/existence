
using UnityEngine;

public class TargetController : GameSystem
{
    public delegate void TargetDelegate(Selectable _s);
    public event TargetDelegate OnPrimaryTargetSelected;
    public event TargetDelegate OnPrimaryTargetDeselected;
    public event TargetDelegate OnSecondaryTargetSelected;
    public event TargetDelegate OnSecondaryTargetDeselected;

#region inputs
    private KeyCode m_CycleKey = KeyCode.Tab;
    private KeyCode m_CancelKey = KeyCode.Escape;
    private bool m_Cycle;
    private bool m_Cancel;
#endregion

#region targets
    private Selectable m_PrimaryTarget;
    private Selectable m_SecondaryTarget;
#endregion

#region Unity Functions
    private void Update() {
        GetInput();
    }
#endregion

#region Private Functions
    private void GetInput() {
        
    }

    private void Cycle() {

    }

    private void Cancel() {

    }
#endregion
}
