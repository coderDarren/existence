
using UnityEngine;
using Tools.Game.Pooling;

public class CustomPoolObjectExample : PoolObject
{
    #region Overridable Functions
        protected virtual void OnObjectEnable() {
            base.OnObjectEnable();
            // you can reference this pool object's pool if needed by accessing 'm_Pool'
        }

        protected virtual void OnObjectDisable() {
            base.OnObjectDisable();
        }
#endregion
}