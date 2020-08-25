
public class NetworkShopTerminalData : NetworkModel {
    public int id;
    public string[] itemData;
}

public class NetworkShopTerminalInteractData : NetworkModel {
    public int id;
    public NetworkShopTerminalInteractData(int _id) {
        id = _id;
    }
}