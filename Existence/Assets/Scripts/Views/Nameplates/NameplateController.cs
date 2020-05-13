using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The NameplateController is responsible for managing all..
/// ..viewable nameplates visible to the camera
/// </summary>
public class NameplateController : MonoBehaviour
{
    public Nameplate nameplate;
    public Transform target;
    public float maxViewableDistance;
    public float fadeDistance;

    private List<Nameplate> m_Nameplates;

#region Unity Functions
    private void Awake() {
        m_Nameplates = new List<Nameplate>();
    }

    private void Update() {
        nameplate.transform.position = Camera.main.WorldToScreenPoint(target.position);
        float _dist = Vector3.Distance(Camera.main.transform.position, target.position);
        nameplate.SetAlpha(1 - (_dist - maxViewableDistance) / fadeDistance);
    }
#endregion

#region Public Functions

#endregion
}
