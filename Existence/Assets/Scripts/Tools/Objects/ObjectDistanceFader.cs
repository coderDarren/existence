using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistanceFader : MonoBehaviour
{
    public bool relativeToPlayer;
    public Transform relativeTo;
    public float beginFadeDistance=15;
    public float fadeLength=5;

    private float m_Distance;
    private float m_Opacity;
    private bool m_IsFading;
    private SkinnedMeshRenderer[] m_SkinRenderers;
    private MeshRenderer[] m_Renderers;

#region Unity Functions
    private void Start() {
        m_SkinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        m_Renderers = GetComponentsInChildren<MeshRenderer>();
        if (relativeToPlayer) {
            relativeTo = GameObject.FindObjectOfType<Player>().transform;
        }

        m_IsFading = true;
        m_Opacity = 0;
        ApplyMaterialBlendModes("Fade");
        ApplyMaterialOpacity(m_Opacity);
    }

    private void Update() {
        CheckIntegrity();
        if (!relativeTo) return;

        m_Distance = Vector3.Distance(relativeTo.position, transform.position);
        if (m_Distance >= beginFadeDistance && !m_IsFading) {
            m_IsFading = true;
            ApplyMaterialBlendModes("Fade");
        } else if (m_Distance < beginFadeDistance && m_IsFading) {
            m_IsFading = false;
            m_Opacity = 1;
            ApplyMaterialBlendModes("Opaque");
        }

        if (m_IsFading) {
            m_Opacity = 1 - (m_Distance - beginFadeDistance) / fadeLength;
            ApplyMaterialOpacity(m_Opacity);
        }
    }
#endregion

#region Private Functions
    private void CheckIntegrity() {
        if (!relativeTo) {
            if (relativeToPlayer) {
                relativeTo = GameObject.FindObjectOfType<Player>().transform;
            }
        }
    }
    
    private void ApplyMaterialBlendModes(string BLEND_MODE) {
        foreach (SkinnedMeshRenderer _smr in m_SkinRenderers) {
            foreach (Material _m in _smr.materials) {
                Utilities.SetMaterialBlendMode(_m, BLEND_MODE);
            }
        }

        foreach (MeshRenderer _mr in m_Renderers) {
            foreach (Material _m in _mr.materials) {
                Utilities.SetMaterialBlendMode(_m, BLEND_MODE);
            }
        }    
    }

    private void ApplyMaterialOpacity(float _opacity) {
        foreach (SkinnedMeshRenderer _smr in m_SkinRenderers) {
            foreach (Material _m in _smr.materials) {
                Utilities.FadeMaterialTo(_m, _opacity);
            }
        }

        foreach (MeshRenderer _mr in m_Renderers) {
            foreach (Material _m in _mr.materials) {
                Utilities.FadeMaterialTo(_m, _opacity);
            }
        }    
    }
#endregion
}
