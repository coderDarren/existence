using System.Collections.Generic;
using UnityEngine;

public class TargetController : GameSystem
{
    public static TargetController instance;

    public delegate void TargetDelegate(Selectable _s);
    public event TargetDelegate OnTargetSelected;
    public event TargetDelegate OnTargetDeselected;

#region external systems
    private NetworkEntityHandler m_EntityHandler;

    // with integrity!
    private NetworkEntityHandler entityHandler {
        get {
            if (!m_EntityHandler) {
                m_EntityHandler = NetworkEntityHandler.instance;
            }
            if (!m_EntityHandler) {
                LogWarning("Trying to access entities, but no instance of NetworkEntityHandler was found.");
            }
            return m_EntityHandler;
        }
    }
#endregion

#region inputs
    private KeyCode m_CycleKey = KeyCode.Tab;
    private KeyCode m_CancelKey = KeyCode.Escape;
    private bool m_Cycle;
    private bool m_Cancel;
#endregion

#region targets
    public int maxTargetCycleSize=2;
    private Selectable m_PrimaryTarget;
    private Selectable m_SecondaryTarget;
    private List<Mob> m_Mobs;
    private List<Mob> m_ClosestMobs;

    public Selectable primaryTarget { get {return m_PrimaryTarget; } }
    public Selectable secondaryTarget { get {return m_SecondaryTarget; } }
#endregion

#region state
    private int m_CycleIndex;
#endregion

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void OnDisable() {
        if (instance == this) {
            instance = null;
        }
    }

    private void Update() {
        if (instance != this) {
            LogError("Too many TargetControllers are in your scene. Remove from "+gameObject.name+" or "+instance.gameObject.name);
            return;
        }

        GetInput();
        ProcessInput();
    }
#endregion

#region Private Functions
    private void GetInput() {
        m_Cycle = Input.GetKeyDown(m_CycleKey);
        m_Cancel = Input.GetKeyDown(m_CancelKey);
    }

    private void ProcessInput() {
        if (m_Cycle) {
            Cycle();
        }

        if (m_Cancel) {
            Cancel();
        }
    }

    private void GetClosestMobs() {
        if (!entityHandler) return;
        if (m_ClosestMobs != null) {
            m_ClosestMobs.Clear();
        } else {
            m_ClosestMobs = new List<Mob>();
        }

        Transform _player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach(Mob _m in entityHandler.mobs) {
            // get the distance from the player to the mob
            float _dist = Vector3.Distance(_player.position, _m.transform.position);

            // continue adding while we are just filling up the closestMobs list
            if (m_ClosestMobs.Count < maxTargetCycleSize) {
                m_ClosestMobs.Add(_m);
                continue;
            }

            // determine if any mobs within closestMobs list need to be replaced
            float _largest = 0;
            int _swapIndex = 0;
            for (int i = 0; i < m_ClosestMobs.Count; i++) {
                float _d = Vector3.Distance(_player.position, m_ClosestMobs[i].transform.position);
                if (_d > _largest) {
                    _largest = _d;
                    _swapIndex = i;
                }
            }
           
            if (_dist < _largest) {
                m_ClosestMobs[_swapIndex] = _m;
            }
        }
    }

    private void Cycle() {
        GetClosestMobs();
        if (m_ClosestMobs.Count == 0) {
            Cancel();
            return;
        }
        
        if (m_CycleIndex >= m_ClosestMobs.Count) {
            m_CycleIndex = m_ClosestMobs.Count - 1;
        }

        m_CycleIndex++;
        if (m_CycleIndex >= m_ClosestMobs.Count) {
            m_CycleIndex = 0;
        }

        if (m_PrimaryTarget != null) {
            TryAction(OnTargetDeselected, m_PrimaryTarget);
        }

        m_PrimaryTarget = (Selectable)m_ClosestMobs[m_CycleIndex];
        TryAction(OnTargetSelected, m_PrimaryTarget);
    }

    private void Cancel() {
        TryAction(OnTargetDeselected, m_PrimaryTarget);
    }

    private void TryAction(TargetDelegate _action, Selectable _s) {
        if (_s == null) return;
        try {
            _action(_s);
        } catch (System.Exception) {}
    }
#endregion
}
