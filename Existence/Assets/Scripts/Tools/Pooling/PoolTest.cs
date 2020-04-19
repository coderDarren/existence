
using UnityEngine;

namespace Tools.Game.Pooling {
    public class PoolTest : MonoBehaviour
    {
        public Pool pool;
        public float spawnRadius;

#region Unity Functions
        private void Update() {
            if (Input.GetMouseButton(0)) {
                GameObject _obj = pool.SpawnObject();
                if (_obj) {
                    Vector3 _pos = _obj.transform.position;
                    _pos.x = Random.Range(-spawnRadius, spawnRadius);
                    _pos.y = Random.Range(-spawnRadius, spawnRadius);
                    _pos.z = _obj.transform.parent.position.z;
                    _obj.transform.position = _pos;
                }
                // get rid of object using pool.DisposeObject(gameObject reference)
            }
        }
#endregion
    }
}
