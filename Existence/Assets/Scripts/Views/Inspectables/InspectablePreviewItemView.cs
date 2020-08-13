
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InspectablePreviewItemView : MonoBehaviour
{

    public Text name;
    public Text lvl;

    private CanvasGroup m_Canvas;

#region Unity Functions
    private void Awake() {
        m_Canvas = GetComponent<CanvasGroup>();
    }
#endregion

#region Public Functions
    public void Open(PreviewItemData _previewItem) {
        name.text = _previewItem.name;
        lvl.text = "LV. "+_previewItem.level;
        m_Canvas.alpha = 1;
    }

    public void Close() {
        m_Canvas.alpha = 0;
    }
#endregion

}
