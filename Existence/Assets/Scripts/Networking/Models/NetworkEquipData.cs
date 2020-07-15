
public class NetworkEquipData : NetworkModel {
    public int itemID;
    public int inventoryLoc;
    public NetworkEquipData(int _itemID, int _inventoryLoc) {
        itemID = _itemID;
        inventoryLoc = _inventoryLoc;
    }
}

public class NetworkEquipSuccessData : NetworkModel {
    public int playerID;
    public int itemID;
    public int inventoryLoc;
    public int inventorySlot;
}