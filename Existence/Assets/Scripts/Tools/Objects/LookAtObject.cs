
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public Transform target;
    public bool lookAtCamera;

#region Unity Functions
    private void Awake() {
        if (lookAtCamera) {
            target = Camera.main.transform;
        }
    }

    private void Update() {
        LookAt();
        LockRotation();
    }
#endregion

#region Private Functions
    private void LookAt() {
        //transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        //transform.LookAt(target);
        transform.rotation = target.rotation;
    }

    private void LockRotation() {
        //transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
    }
#endregion
}
