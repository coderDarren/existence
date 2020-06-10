
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootPreviewItem : InspectablePreviewItem, IPointerClickHandler
{
    public Image icon;

    private Session m_Session;
    private Mob m_Mob;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Unity Functions
    private void OnEnable() {
        if (!session) return;
        if (!session.network) return;
        session.network.OnMobLooted += OnMobLooted;
    }

    private void OnDisable() {
        if (!session) return;
        if (!session.network) return;
        session.network.OnMobLooted -= OnMobLooted;
    }
#endregion

#region Public Functions
    public void Init(Mob _mob, NetworkLootPreviewData _data) {
        m_Mob = _mob;
        m_PreviewItem = ConstructPreviewItem(_data);
        icon.sprite = Utilities.LoadStreamingAssetsSprite(m_PreviewItem.icon);
    }
#endregion

#region Private Functions
    private PreviewItemData ConstructPreviewItem(NetworkLootPreviewData _data) {
        PreviewItemData _previewItem = new PreviewItemData();
        _previewItem.id = _data.id;
        _previewItem.name = _data.name;
        _previewItem.level = _data.level;
        _previewItem.icon = _data.icon;
        return _previewItem;
    }

    private void OnMobLooted(NetworkMobLootData _data) {
        if (!m_Mob) return;
        if (m_PreviewItem == null) return;
        if (m_Mob.data.id == _data.mobID && m_PreviewItem.id == _data.itemID) {
            Destroy(gameObject);
        }
    }
#endregion

#region Interface Functions
    public void OnPointerClick(PointerEventData _ped) {
        if (!session) return;
        if (!session.network) return;
        NetworkPlayerLootData _lootData = new NetworkPlayerLootData(m_Mob.data.id, m_PreviewItem.id);
        session.network.LootMob(_lootData);
    }
#endregion
}
