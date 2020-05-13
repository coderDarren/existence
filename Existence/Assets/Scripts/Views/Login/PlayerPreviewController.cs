
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerPreviewController : MonoBehaviour
{
    private Animator m_Animator;

#region Unity Function
    private void Awake() {
        m_Animator = GetComponent<Animator>();
    }
#endregion
}
