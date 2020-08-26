using System.Collections.Generic;
using UnityEngine;

public class ShopTerminal : Selectable
{
    [Header("NetworkInfo")]
    public int id;

    [Header("Display")]
    public string name;

    private ShopManager m_ShopManager;

    // get ShopManager with integrity
    private ShopManager shopManager {
        get {
            if (!m_ShopManager) {
                m_ShopManager = ShopManager.instance;
            }
            if (!m_ShopManager) {
                LogError("Trying to use ShopManager, but no instance could be found.");
            }
            return m_ShopManager;
        }
    }

#region Unity Functions
    private void Start() {
        m_NameplateData = new NameplateData();
        m_NameplateData.name = name;
        NameplateController.instance.TrackSelectable(this);
    }

    private void OnDisable() {
        NameplateController.instance.ForgetSelectable(this);
    }
#endregion

#region Public Functions
    
#endregion

#region Override Functions
    public override void OnSelected() {
        base.OnSelected();
        Log("you selected shop terminal "+id);
        if (!shopManager) return;
        shopManager.OpenShop(id);
    }

    public override void OnDeselected() {
        base.OnDeselected();
        Log("you deselected shop terminal "+id);
    }
#endregion
}
