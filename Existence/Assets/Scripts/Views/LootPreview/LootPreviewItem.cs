
using UnityEngine;
using UnityEngine.UI;

public class LootPreviewItem : MonoBehaviour
{
    public Image icon;

#region Public Functions
    public void Init(NetworkLootPreviewData _data) {
        icon.sprite = Utilities.LoadStreamingAssetsSprite(_data.icon);
    }
#endregion
}
