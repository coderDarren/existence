
using UnityEngine;
using UnityEngine.UI;

/*
 * Defines a loot preview section specific to a particular mob.
 * This object is responsible for displaying a mob's..
 * ..individual loot preview items
 */
public class LootPreviewMobSection : MonoBehaviour
{
    public GameObject PreviewItem;
    public RectTransform itemContainer; 
    public Text name;
    public Text distance;

    private Transform m_Player;
    private Mob m_Mob;
    private GridLayoutGroup m_Layout;
    private UIContainer m_MasterContainer;

#region Unity Functions
    private void OnDisable() {
        m_MasterContainer.OnResize -= OnMasterContainerResize;
    }

    private void Update() {
        if (m_Mob == null) return;
        if (!m_Player) return;
        int _dist = (int)Vector3.Distance(m_Player.position, m_Mob.transform.position);
        float _progress = _dist / 25.0f;
        distance.text = _dist+"m";
        //distance.color = Color.Lerp(Color.green, Color.red, _progress);
        distance.color = _dist > 10 ? Color.red : Color.green;
    }
#endregion

#region Public Functions
    public void Init(UIContainer _masterContainer, Mob _mob) {
        m_MasterContainer = _masterContainer;
        m_Layout = transform.parent.GetComponent<GridLayoutGroup>();
        m_MasterContainer.OnResize += OnMasterContainerResize;
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_Mob = _mob;

        int _dist = (int)Vector3.Distance(m_Player.position, m_Mob.transform.position);
        float _progress = _dist / 10.0f;
        distance.text = _dist+"m";
        distance.color = Color.Lerp(Color.green, Color.red, _progress);
        name.text = _mob.data.name;
        int i = 0;
        foreach(NetworkLootPreviewData _data in _mob.data.lootPreview) {
            GameObject _obj = Instantiate(PreviewItem);
            LootPreviewItem _preview = _obj.GetComponent<LootPreviewItem>();
            RectTransform _rect = _obj.GetComponent<RectTransform>();
            _rect.SetParent(itemContainer);
            _rect.localScale = Vector3.one;
            _preview.Init(m_Mob, _data);
            UpdateContainerWidth(i * 60 + i * 8);
        }
    }
#endregion

#region Private Functions
    private void UpdateContainerWidth(float _width) {
        Vector2 _size = itemContainer.sizeDelta;
        _size.x = _width;
        itemContainer.sizeDelta = _size;
    }

    private void UpdateLayoutWidth(float _width) {
        Vector2 _size = m_Layout.cellSize;
        _size.x = _width;
        m_Layout.cellSize = _size;
    }

    private void OnMasterContainerResize(Vector2 _size) {
        UpdateLayoutWidth(_size.x - 35);
    }
#endregion
}
