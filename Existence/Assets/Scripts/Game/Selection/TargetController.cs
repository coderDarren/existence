
using UnityEngine;

public class TargetController : GameSystem
{
    public delegate void TargetDelegate(Selectable _s);
    public event TargetDelegate OnPrimaryTargetSelected;
    public event TargetDelegate OnPrimaryTargetDeselected;
    public event TargetDelegate OnSecondaryTargetSelected;
    public event TargetDelegate OnSecondaryTargetDeselected;

    private KeyCode m_CycleInput = KeyCode.Tab;
    private KeyCode m_Cancel = KeyCode.Escape;

#region Unity Functions
    private void Update() {
        
    }
#endregion
}
