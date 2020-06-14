using System.Collections.Generic;
using UnityEngine;

public class TargetController : GameSystem
{
    public static TargetController instance;

    public delegate void TargetDelegate(Selectable _s, bool _primary);
    public event TargetDelegate OnTargetSelected;
    public event TargetDelegate OnTargetDeselected;

    [Header("Cycle Selection")]
    public int maxTargetCycleSize=2;

    //[Header("Mouse Selection")]
    

#region external systems
    private PlayerCombatController m_PlayerCombat;
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

    // with integrity!
    private PlayerCombatController playerCombat {
        get {
            if (!m_PlayerCombat) {
                m_PlayerCombat = PlayerCombatController.instance;
            }
            if (!m_PlayerCombat) {
                LogWarning("Trying to access player combat, but no instance of PlayerCombatController was found.");
            }
            return m_PlayerCombat;
        }
    }
#endregion

#region inputs
    private KeyCode m_CycleKey = KeyCode.Tab;
    private KeyCode m_CancelKey = KeyCode.Escape;
    private bool m_LMB;
    private bool m_Cycle;
    private bool m_Cancel;
#endregion

#region targets
    private Selectable m_PrimaryTarget;
    private Selectable m_SecondaryTarget;
    private Selectable m_OtherTarget;
    private List<Mob> m_Mobs;
    private List<Mob> m_ClosestMobs;

    public Selectable primaryTarget { get {return m_PrimaryTarget; } }
    public Selectable secondaryTarget { get {return m_SecondaryTarget; } }
    public Selectable otherTarget { get {return m_OtherTarget; } }
#endregion

#region state
    private int m_CycleIndex=-1;
#endregion

    private RaycastHit m_MouseHitInfo;

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void Start() {
        if (instance != this) return;

        if (entityHandler) {
            entityHandler.OnMobDidDie += OnMobDidDie;
        }
    }

    private void OnDisable() {
        if (instance == this) {
            instance = null;
            if (entityHandler) {
                entityHandler.OnMobDidDie -= OnMobDidDie;
            }
        }
    }

    private void Update() {
        if (instance != this) {
            LogError("Too many TargetControllers are in your scene. Remove from "+gameObject.name+" or "+instance.gameObject.name);
            return;
        }

        GetInput();
        ProcessInput();
        UpdateTargetState();
    }
#endregion

#region Private Functions
    private void GetInput() {
        m_Cycle = Input.GetKeyDown(m_CycleKey);
        m_Cancel = Input.GetKeyDown(m_CancelKey);
        m_LMB = Input.GetMouseButtonDown(0);
    }

    private void ProcessInput() {
        if (m_Cycle) {
            TabCycleTargets();
        }

        if (m_LMB) {
            MouseSelectTarget();
        }

        if (m_Cancel) {
            Cancel();
        }
    }

    private void MouseSelectTarget() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_MouseHitInfo)) {
            Selectable _s = m_MouseHitInfo.collider.gameObject.GetComponent<Selectable>();
            if (!_s) return;

            // is this selection a mob?
            if (_s.GetType().IsAssignableFrom(typeof(Mob))) {
                // do not allow selection of dead mobs
                if (((Mob)_s).data.dead) {
                    return;
                }
                if (m_PrimaryTarget != null) {
                    TryAction(OnTargetDeselected, m_PrimaryTarget, true);
                }
                m_PrimaryTarget = _s;
                TryAction(OnTargetSelected, m_PrimaryTarget, true);
            }
            // for now, other targets are considered anything other than mobs - such as players
            // this way, combat isn't interrupted when clicking other selectables 
            else {
                if (m_OtherTarget != null) {
                    TryAction(OnTargetDeselected, m_OtherTarget, false);
                }
                m_OtherTarget = _s;
                TryAction(OnTargetSelected, m_OtherTarget, false);
            }
        }
    }

    private void TabCycleTargets() {
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

        // update the primary target if the player is not attacking
        if (!playerCombat.attacking) {
            // deselect the last primary target if needed
            if (m_PrimaryTarget != null) {
                TryAction(OnTargetDeselected, m_PrimaryTarget, true);
            }
            m_PrimaryTarget = (Selectable)m_ClosestMobs[m_CycleIndex];
            TryAction(OnTargetSelected, m_PrimaryTarget, true);
        } 
        // otherwise, update the secondary target as long as this cycle is not on the primary target
        else {
            // cycle one more time if we are targeting the primary target
            if ((Selectable)m_ClosestMobs[m_CycleIndex] == m_PrimaryTarget) {
                m_CycleIndex++;
                if (m_CycleIndex >= m_ClosestMobs.Count) {
                    m_CycleIndex = 0;
                }  
            }

            // if we are still on primary target, exit
            if ((Selectable)m_ClosestMobs[m_CycleIndex] == m_PrimaryTarget) {
                return;
            }

            // deselect the last secondary target if needed
            if (m_SecondaryTarget != null) {
                TryAction(OnTargetDeselected, m_SecondaryTarget, false);
            }
            m_SecondaryTarget = (Selectable)m_ClosestMobs[m_CycleIndex];
            TryAction(OnTargetSelected, m_SecondaryTarget, false);
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
            // ignore dead mobs
            if (_m.data.dead) {
                continue;
            }

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

    private void Cancel() {
        TryAction(OnTargetDeselected, m_PrimaryTarget, true);
        TryAction(OnTargetDeselected, m_SecondaryTarget, false); 
        TryAction(OnTargetDeselected, m_OtherTarget, false);
    }

    private void UpdateTargetState() {
        if (m_PrimaryTarget != null) {
            if (((Mob)m_PrimaryTarget).data.dead) {
                if (m_SecondaryTarget != null) {
                    SwitchToSecondaryTarget();
                } else {
                    m_PrimaryTarget = null;
                    TryAction(OnTargetDeselected, m_PrimaryTarget, true);
                }
            }
        } else if (m_SecondaryTarget != null) {
            SwitchToSecondaryTarget();
        }
    }

    private void SwitchToSecondaryTarget() {
        m_PrimaryTarget = m_SecondaryTarget;
        m_SecondaryTarget = null;
        TryAction(OnTargetSelected, m_PrimaryTarget, true);
        TryAction(OnTargetDeselected, m_SecondaryTarget, false);
    }

    private void OnMobDidDie(Mob _m) {
        TryAction(OnTargetDeselected, (Selectable)_m, false);
    }

    private void TryAction(TargetDelegate _action, Selectable _s, bool _primary) {
        if (_s == null) return;
        try {
            _action(_s, _primary);
        } catch (System.Exception) {}
    }
#endregion
}
