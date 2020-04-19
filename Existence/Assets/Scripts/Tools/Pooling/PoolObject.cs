
using UnityEngine;

namespace Tools.Game.Pooling {
    public class PoolObject : MonoBehaviour
    {

        protected Pool m_Pool;

#region Public Functions
        public void Enable() {
            OnObjectEnable();
        }

        public void Disable() {
            OnObjectDisable();
        }
#endregion

#region Overridable Functions
        protected virtual void OnObjectEnable() {
            m_Pool = transform.parent.GetComponent<Pool>();

            if (!m_Pool) {
                Debug.LogWarning("Pool object does not have a valid reference to its pool.");
            }
        }

        protected virtual void OnObjectDisable() {

        }
#endregion
    }
}
