
using UnityEngine;

[CreateAssetMenuAttribute(menuName="Pathfinding/Graph Data", fileName="New Graph Data")]
public class GraphData : ScriptableObject
{
    [System.Serializable]
    public class Node {
        public Transform transform;
        public Node[] neighbors;
    }

    public Node graph;
}
