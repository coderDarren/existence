
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GraphNodeHandle : MonoBehaviour
{
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(GraphNodeHandle))]
public class GraphNodeHandleEditor : Editor {
    private Transform m_Obj;
    private GraphNodeHandle m_Script;
    private PathfindingVisualizer m_Visualizer;
    private Node m_Node;

    private void OnEnable() {
        m_Script = (GraphNodeHandle)target;
        m_Obj = m_Script.gameObject.transform;
        if (m_Obj.parent) {
            m_Visualizer = m_Obj.parent.GetComponent<PathfindingVisualizer>();
        }
    }

    public override void OnInspectorGUI() {
        if (GUILayout.Button("Add This")) {
            AddThis();
        }

        if (GUILayout.Button("Add New")) {
            AddNew();
        }
    }

    private void AddThis() {
        if (m_Obj.parent) {
            m_Node = new Node(m_Obj.gameObject.GetInstanceID(), new NetworkVector3(m_Obj.position.x, m_Obj.position.y, m_Obj.position.z));
            m_Visualizer.Add(m_Node);
        }
    }

    private void AddNew() {
        if (m_Obj.parent) {
            GameObject _go = Instantiate(m_Visualizer.GraphNodeHandle, m_Obj.position + Vector3.right, Quaternion.identity);
            _go.transform.parent = m_Obj.parent;
            int _goId = _go.GetInstanceID();
            int _id1 = m_Node.objectId;
            int _id2 = _goId;
            m_Visualizer.Add(new Node(_goId, new NetworkVector3(_go.transform.position.x, _go.transform.position.y, _go.transform.position.z)));
            m_Visualizer.Link(_id1, _id2);
        }
    }
}
#endif