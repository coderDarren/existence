using System.Collections.Generic;
using UnityEngine;

public class Node : NetworkModel {
    public int objectId;
    public NetworkVector3 pos;
    public List<Edge> edges;
    public Node(int _objectId, NetworkVector3 _pos) {
        edges = new List<Edge>();
        objectId = _objectId;
        pos = _pos;
    }
    public void AddEdge(Node _to) {
        edges.Add(new Edge(this, _to));
    }
}

public class Edge : NetworkModel {
    public Node from;
    public Node to;
    public float length;
    public Edge(Node _from, Node _to) {
        from = _from;
        to = _to;
        length = Vector3.Distance(new Vector3(_from.pos.x,_from.pos.y,_from.pos.z), new Vector3(_to.pos.x,_to.pos.y,_to.pos.z));
    }
}

public class Graph : NetworkModel {
    public List<Node> nodes;

    public void AddNode(Node _node) {
        if (nodes == null) {
            nodes = new List<Node>();
        }
        nodes.Add(_node);
    }

    public void LinkNodes(Node _from, Node _to) {
        _from.AddEdge(_to);
        _to.AddEdge(_from);
    }
}