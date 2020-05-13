
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Nameplate : GameSystem
{
        
    private CanvasGroup m_Canvas;

#region Unity Functions
    private void Awake() {
        m_Canvas = GetComponent<CanvasGroup>();
    }
#endregion

#region Public Functions
    public void SetAlpha(float _alpha) {
        m_Canvas.alpha = _alpha;
    }
#endregion

#region Private Functions

#endregion
}
