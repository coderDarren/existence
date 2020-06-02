
using UnityEngine;
using UnityEngine.UI;

public class CursorController : GameSystem
{
    public static CursorController instance;

    public Image cursor;
    public Image pickup;
    public InspectableItemView inspectableItemView;
    public Sprite main;
    public Sprite scaleHorizontal;
    public Sprite scaleVertical;
    public Sprite scaleDiagRight;
    public Sprite scaleDiagLeft;
    public Sprite drag;

    private RectTransform m_Rect;
    private ItemData m_SelectedItem;

    public ItemData selectedItem {
        get {
            return m_SelectedItem;
        }
    }

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
            Cursor.visible = false;
            m_Rect = cursor.GetComponent<RectTransform>();
            LoadMain();
        }
    }

    private void Update() {
        Vector3 _cursorPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        _cursorPos.x *= Screen.width;
        _cursorPos.y *= Screen.height;
        transform.position = _cursorPos;
    }
#endregion

#region Private Functions
    private void SetImageAlpha(Image _img, float _alpha) {
        Color _c = _img.color;
        _c.a = _alpha;
        _img.color = _c;
    }
#endregion

#region Public Functions
    public void LoadMain() {
        cursor.sprite = main;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.up;
        m_Rect.localScale = Vector2.one;
    }

    public void LoadScaleHorizontal() {
        cursor.sprite = scaleHorizontal;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.one * 0.5f;
        m_Rect.localScale = Vector2.one * 1.35f;
    }

    public void LoadScaleVertical() {
        cursor.sprite = scaleVertical;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.one * 0.5f;
        m_Rect.localScale = Vector2.one * 1.35f;
    }

    public void LoadScaleDiagRight() {
        cursor.sprite = scaleDiagRight;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.one * 0.5f;
        m_Rect.localScale = Vector2.one * 1.35f;
    }

    public void LoadScaleDiagLeft() {
        cursor.sprite = scaleDiagLeft;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.one * 0.5f;
        m_Rect.localScale = Vector2.one * 1.35f;
    }

    public void LoadDrag() {
        cursor.sprite = drag;
        cursor.material.mainTexture = cursor.sprite.texture;
        m_Rect.pivot = Vector2.one * 0.5f;
        m_Rect.localScale = Vector2.one * 1.35f;
    }

    public void SelectItem(ItemData _item) {
        m_SelectedItem = _item;
        pickup.sprite = Utilities.LoadStreamingAssetsSprite(m_SelectedItem.icon);
        SetImageAlpha(pickup, 1);
    }

    public void DropItem() {
        m_SelectedItem = null;
        SetImageAlpha(pickup, 0);
    }

    public void OpenHoverItem(ItemData _item) {
        inspectableItemView.Open(_item);
    }

    public void CloseHoverItem() {
        inspectableItemView.Close();
    }
#endregion
}
