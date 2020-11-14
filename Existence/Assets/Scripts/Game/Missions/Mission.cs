using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : GameSystem
{
    public string missionId;
    public string nodeId;

    private Session m_Session;
    private IMissionNode m_ActiveNode;
    private IMissionNode[] m_NextNodes;

    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogWarning("Trying to access session, but no instance could be found.");
            }
            return m_Session;
        }
    }

    public void Interact(IMissionInteraction _data) {
        if (!session) return;
        session.network.InteractMission(_data);
    }

    protected virtual void GenerateNextData() {
        if (m_ActiveNode == null) {
            LogWarning("[GenerateNextData]: Unable to generate data. No active node exists.");
            return;
        }
    }
}
