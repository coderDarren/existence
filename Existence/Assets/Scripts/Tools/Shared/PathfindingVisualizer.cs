
using UnityEngine;

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
        if (Application.isPlaying) return;
        Draw();
    }
#endregion

#region Private Functions
    private void Draw() {
        foreach (Transform _t in transform) {
            Log("drawing node");
            if (_t == transform) continue;
            GraphNode _node = _t.GetComponent<GraphNode>();
            if (_node == null) return;
            DrawNode(_node);
            Log("drawing node");
        }
    }

    private void DrawNode(GraphNode _node) {
        foreach (GraphNode _neighbor in _node.neighbors) {
            DrawEdge(_node.transform.position, _neighbor.transform.position);
            DrawNode(_neighbor);
        }
    }

    private void DrawEdge(Vector3 _pos1, Vector3 _pos2) {
        Debug.DrawLine(_pos1, _pos2, Color.green);
    }
#endregion
}
