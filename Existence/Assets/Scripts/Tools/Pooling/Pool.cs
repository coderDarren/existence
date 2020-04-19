using System.Collections.Generic;
using UnityEngine;

namespace Tools.Game.Pooling {
    public class Pool : MonoBehaviour
    {

        public int poolSize;
        public GameObject referenceObject;
        public Vector3 disposePos = new Vector3(10000,10000,-10000);

        private Queue<GameObject> m_Objects;

#region Unity Functions
        private void OnEnable() {
            CreatePool();
        }

        private void OnDisable() {
            DisposePool();
        }
#endregion

#region Public Functions
        public GameObject SpawnObject() {
            if (m_Objects == null || m_Objects.Count == 0){
                Debug.LogWarning("Unable to spawn object. Pool is not initialized.");
                return null;   
            }

            // Search for the next object in line
            GameObject _obj = m_Objects.Peek();
            int _search = 0;
            // Keep searching for the next in line, kicking busy objects to the end of the line
            while (_search < m_Objects.Count && _obj.activeSelf) {
                // kick object to the end of the line
                m_Objects.Dequeue();
                m_Objects.Enqueue(_obj);
                _obj = m_Objects.Peek();
                _search++;
            }

            // If no objects are available, you probably need a larger pool size
            if (_obj.activeSelf) {
                Debug.LogWarning("Unable to spawn object. Pool is busy. You may want to consider increasing your pool size.");
                return null;
            }

            // Use this object and kick it to the end of the line
            m_Objects.Dequeue();
            m_Objects.Enqueue(_obj);
            _obj.SetActive(true);
            PoolObject _po = _obj.GetComponent<PoolObject>();
            if (_po) {
                _po.Enable();
            }
            return _obj;
        }

        public void DisposeObject(GameObject _obj) {
            _obj.SetActive(false);
            PoolObject _po = _obj.GetComponent<PoolObject>();
            if (_po) {
                _po.Disable();
            }
            _obj.transform.position = disposePos;
        }
#endregion

#region Private Functions
        private void CreatePool() {
            if (m_Objects != null) return;
            m_Objects = new Queue<GameObject>();

            referenceObject.SetActive(false);
            m_Objects.Enqueue(referenceObject);

            for (int i = 0; i < poolSize; i++) {
                GameObject _obj = Instantiate(referenceObject);
                _obj.transform.parent = transform;
                _obj.transform.position = disposePos;
                if (!_obj.GetComponent<PoolObject>()) {
                    _obj.AddComponent(typeof(PoolObject));
                }
                _obj.SetActive(false);
                m_Objects.Enqueue(_obj);
            }
        }

        private void DisposePool() {
            if (m_Objects == null) return;
        }
#endregion
    }
}
