using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// PathfindingVisualizer is used to create a visual representation of node-edge based graph
/// Right now, we use this to help us visualize graphs for server pathfinding
/// </summary>
[ExecuteInEditMode]
public class PathfindingVisualizer : GameSystem
{
    public GameObject GraphNodeHandle;
    public Graph graph;

    private Hashtable m_NodeHash;
    private Hashtable m_VisitedNodes;
    
#region Unity Functions
    /*private void Update() {
        if (!debug) return;
        if (Application.isPlaying) return;
        m_NodeHash = new Hashtable();
        Draw(transform);
    }*/
#endregion

#region Public Functions
    public void Add(Node _node) {
        if (graph == null) {
            graph = new Graph();
        }
        if (m_NodeHash == null) {
            m_NodeHash = new Hashtable();
        }
        
        if (m_NodeHash.ContainsKey(_node.objectId)) {
            Debug.Log("node already registered: "+_node.objectId);
            return;
        }

        m_NodeHash.Add(_node.objectId, _node);
        graph.AddNode(_node);
    }

    public void Link(int _obj1, int _obj2) {
        if (!m_NodeHash.ContainsKey(_obj1)) {
            Log("No node found with id: "+_obj1);
            return; 
        }
        if (!m_NodeHash.ContainsKey(_obj2)) {
            Log("No node found with id: "+_obj2);
            return;
        }

        Node _n1 = (Node)m_NodeHash[_obj1];
        Node _n2 = (Node)m_NodeHash[_obj2];
        graph.LinkNodes(_n1, _n2);

        m_VisitedNodes = new Hashtable();
        UpdateVisuals(graph.nodes[0]);
    }

    public void UpdateVisuals(Node _n) {
        if (!m_VisitedNodes.ContainsKey(_n.objectId)) {
            m_VisitedNodes.Add(_n.objectId, _n);
        } else {
            return;
        }

        foreach (Edge _e in _n.edges) {
            Debug.Log("drawing edge");
            Debug.DrawLine(new Vector3(_e.from.pos.x,_e.from.pos.y,_e.from.pos.z),new Vector3(_e.to.pos.x,_e.to.pos.y,_e.to.pos.z), Color.green);
            UpdateVisuals(_e.to);
        }
    }
#endregion

#region Private Functions
    
#endregion
}

[CustomEditor(typeof(PathfindingVisualizer))]
public class PathfindingVisualizerEditor : Editor {
    SerializedProperty graphNodeHandle;

    private void OnEnable() {
        graphNodeHandle = serializedObject.FindProperty("GraphNodeHandle");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField (graphNodeHandle, new GUIContent ("GraphNodeHandle"));
        serializedObject.ApplyModifiedProperties();
    }
}
