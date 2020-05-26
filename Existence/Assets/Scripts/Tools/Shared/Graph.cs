using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Node : NetworkModel {
    public string id;
    public NetworkVector3 pos;
    public List<Edge> paths;
    private Hashtable m_EdgeHash;

    public Node(NetworkVector3 _pos) {
        paths = new List<Edge>();
        m_EdgeHash = new Hashtable();
        id = _pos.x+"-"+_pos.y+"-"+_pos.z;
        pos = _pos;
    }
    public void AddEdge(Node _to) {
        string _key = _to.pos.x+"-"+_to.pos.y+"-"+_to.pos.z;
        if (m_EdgeHash.ContainsKey(_key)) {
            return;
        }
        float _length = Vector3.Distance(new Vector3(pos.x,pos.y,pos.z), new Vector3(_to.pos.x,_to.pos.y,_to.pos.z));
        Edge _e = new Edge(id, _to.id, _length);
        paths.Add(_e);
        m_EdgeHash.Add(_key, _to);
    }
}

public class Edge : NetworkModel {
    public string from;
    public string to;
    public float length;
    public Edge(string _from, string _to, float _length) {
        from = _from;
        to = _to;
        length = _length;
    }
}

public class Graph : NetworkModel {
    public List<Node> waypoints;
    private Hashtable m_NodeHash;

    public Graph() {
        waypoints = new List<Node>();
        m_NodeHash = new Hashtable();
    }

    public int AddNode(Node _node) {
        string _key = _node.id;
        if (m_NodeHash.ContainsKey(_key)) {
            //Debug.Log("Node already exists: "+_key);
            return IndexAt(_key);
        }
        waypoints.Add(_node);
        m_NodeHash.Add(_key, _node);
        return waypoints.Count - 1;
    }

    public void LinkNodes(int _from, int _to) {
        waypoints[_from].AddEdge(waypoints[_to]);
        waypoints[_to].AddEdge(waypoints[_from]);
    }

    private int IndexAt(string _id) {
        for (int i = 0; i < waypoints.Count; i++) {
            if (waypoints[i].id == _id) {
                return i;
            }
        }
        return -1;
    }
}