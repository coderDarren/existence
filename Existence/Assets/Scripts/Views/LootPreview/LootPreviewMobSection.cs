
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

#region Public Functions
    public void Init(Mob _mob) {
        name.text = _mob.data.name;
        int i = 0;
        foreach(NetworkLootPreviewData _data in _mob.data.lootPreview) {
            GameObject _obj = Instantiate(PreviewItem);
            LootPreviewItem _preview = _obj.GetComponent<LootPreviewItem>();
            RectTransform _rect = _obj.GetComponent<RectTransform>();
            _rect.SetParent(itemContainer);
            _rect.localScale = Vector3.one;
            _preview.Init(_data);
            UpdateContainerWidth(i * 70 + i * 8);
        }
    }
#endregion

#region Private Functions
    private void UpdateContainerWidth(float _width) {
        Vector2 _size = itemContainer.sizeDelta;
        _size.x = _width;
        itemContainer.sizeDelta = _size;
    }
#endregion
}
