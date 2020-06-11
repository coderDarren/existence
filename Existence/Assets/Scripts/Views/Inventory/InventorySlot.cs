
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Derive InspectableItem to access the cursor, the item data, and hover events
 */
public class InventorySlot : InspectableItem
{
    /*
     * Create an event system for all inventory slots to listen to
     * .. this should include bags in the future...
     *
     * When a grab or drop event occurs, each inventory slot will be able to..
     * ..decide how to redraw their own slot
     */
    private delegate void InventoryAction(InventorySlot _slot);
    private static event InventoryAction OnGrab;
    private static event InventoryAction OnDrop;

    // store reference to the image that displays this slots item
    public Image icon;
    public InventoryPage controller;

    // keep an id that will refer to the db inventory slot id
    private int m_Id;
    // keep track of the currently grabbed slot
    private InventorySlot m_GrabbedSlot;

    public int id {
        get {
            return m_Id;
        }
    }

#region Public Functions
    public void Init(int _id) {
        m_Id = _id;
        InventorySlot.OnGrab += OnSomeSlotGrab;
        InventorySlot.OnDrop += OnSomeSlotDrop;
    }

    public void EraseIcon() {
        UpdateAlpha(0);
        icon.sprite = null;
        m_Item = null;
    }

    public void AssignIcon(ItemData _item) {
        m_Item = _item;
        m_Item.slotLoc = id;
        Sprite _sprite = Utilities.LoadStreamingAssetsSprite(m_Item.icon);
        if (_sprite == null) return;

        UpdateAlpha(1);
        icon.sprite = _sprite;
    }

    public void HandleAction() {
        if (m_Item == null) {
            // slot is clear
            TryInventoryAction(OnDrop);
        } else {
            // slot is full
            TryInventoryAction(OnGrab);
        }

    }
#endregion

#region Private Functions
    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }

    /*
     * Straightforward grab function
     * ..if the cursor exists and there is an item on the slot..
     * ..use the cursor to select the item
     */
    private void OnSomeSlotGrab(InventorySlot _slot) {
        if (_slot == this) {
            if (!cursor) return;
            if (m_Item == null) return;
            cursor.SelectItem(m_Item);
        } else {
            // remember the slot that was grabbed
            if (_slot.item == null) return; // nothing was actually grabbed..
            m_GrabbedSlot = _slot;
        }
    }

    /*
     * Here you can do a couple of things
     * 1. Drop items into empty slots
     * 2. Stack items
     * 3. Swap items
     */
    private void OnSomeSlotDrop(InventorySlot _slot) {
        if (_slot == this) {
            if (!m_GrabbedSlot) return;
            if (!cursor) return;
            if (cursor.selectedItem == null) return;
            if (m_Item != null) return;
            AssignIcon(cursor.selectedItem);
            cursor.DropItem();
            m_GrabbedSlot.EraseIcon();
            controller.SaveInventory(m_Item);
        } else {
            
        }

        // all inventory slots should forget recently grabbed slots
        m_GrabbedSlot = null;
    }

    private void TryInventoryAction(InventoryAction _action) {
        try {
            _action(this);
        } catch (System.Exception _e) {
            Debug.LogWarning(_e);
        }
    }
#endregion

#region Override Functions
    protected override void Dispose() {
        base.Dispose();
        InventorySlot.OnGrab -= OnSomeSlotGrab;
        InventorySlot.OnDrop -= OnSomeSlotDrop;
    }
#endregion

}
