

public class NetworkInventoryUpdate : NetworkModel
{
    public int slotID;
    public int slotLoc;
    public NetworkInventoryUpdate(int _slotId, int _slotLoc) {
        slotID = _slotId;
        slotLoc = _slotLoc;
    }
}

public class NetworkInventoryRemoveData : NetworkModel 
{
    public int itemID;
    public int inventoryLoc;
}