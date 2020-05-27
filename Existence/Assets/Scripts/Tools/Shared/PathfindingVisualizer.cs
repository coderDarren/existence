using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Tools.IO;
#endif

/// <summary>
/// PathfindingVisualizer is used to create a visual representation of node-edge based graph
/// Right now, we use this to help us visualize graphs for server pathfinding
/// </summary>
[ExecuteInEditMode]
public class PathfindingVisualizer : GameSystem
{
    
#region Unity Functions
    private void Update() {
        if (!debug) return;
        //if (Application.isPlaying) return;
        Draw();
    }
#endregion

#region Private Functions
    private void Draw() {
        foreach (Transform _t in transform) {
            if (_t == transform) continue;
            GraphNode _node = _t.GetComponent<GraphNode>();
            if (_node == null) return;
            DrawNode(_node);
        }
    }

    private void DrawNode(GraphNode _node) {
        foreach (GraphNode _neighbor in _node.neighbors) {
            if (!_neighbor) continue;
            DrawEdge(_node.transform.position, _neighbor.transform.position);
            DrawNode(_neighbor);
        }
    }

    private void DrawEdge(Vector3 _pos1, Vector3 _pos2) {
        Debug.DrawLine(_pos1, _pos2, Color.green);
    }
#endregion
}

[CustomEditor(typeof(PathfindingVisualizer))]
public class PathfindingVisualizerEditor : Editor {
    public SerializedProperty debug;
    private Transform m_Obj;

    private void OnEnable() {
        debug = serializedObject.FindProperty("debug");
        m_Obj = ((PathfindingVisualizer)target).transform;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(debug, new GUIContent("Draw Graph"));
        if (GUILayout.Button("Export Graph")) {
            Debug.Log("Building graph from children of "+m_Obj.gameObject.name);
            Graph _graph = new Graph();
            foreach (Transform _t in m_Obj) {
                GraphNode _node = _t.GetComponent<GraphNode>();
                if (_node == null) continue;
                BuildGraph(ref _graph, _node);
                break;
            }
            string _json = _graph.ToJsonString();
            Debug.Log("Graph finished building with result: "+_json);
            SaveGraph(_graph);
        }
        serializedObject.ApplyModifiedProperties(); 
    }

    private void BuildGraph(ref Graph _graph, GraphNode _graphNode) {
        Node _parent = new Node(new NetworkVector3(_graphNode.transform.position.x, _graphNode.transform.position.y, _graphNode.transform.position.z));
        Debug.Log("Building node "+_parent.id);
        int _parentIndex = _graph.AddNode(_parent);
        if (_parentIndex == -1) {
            Debug.Log("Parent node was already added");
            return;
        }
        foreach (GraphNode _n in _graphNode.neighbors) {
            Debug.Log(_n.gameObject.transform);
            Node _node = new Node(new NetworkVector3(_n.transform.position.x, _n.transform.position.y, _n.transform.position.z));
            Debug.Log("Branching node "+_node.id);
            int _childIndex = _graph.AddNode(_node);
            if (_childIndex == -1) {
                Debug.Log("Child node was already added");
                continue;
            }
            Debug.Log("Linking nodes "+_parent.id+" <=> "+_node.id);
            _graph.LinkNodes(_parentIndex, _childIndex);
            BuildGraph(ref _graph, _n);
        }
    }

    private void SaveGraph(Graph _graph) {
        string fileName = Application.dataPath+"/Resources/AIWaypointGraphs/save.json";
        Files.CreateDirectoryIfNull(Application.dataPath+"/Resources");
        Files.CreateDirectoryIfNull(Application.dataPath+"/Resources/AIWaypointGraphs");
        Files.CreateFileIfNull(fileName, string.Empty);
        Files.WriteToFile(fileName, _graph.ToJsonString(), false);
    }
}