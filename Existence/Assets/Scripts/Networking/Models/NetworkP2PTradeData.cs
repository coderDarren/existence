
public class NetworkP2PTradeData : NetworkModel {
    public bool accepted;
    public string[] outgoingItems;
    public string[] incomingItems;
}

public class NetworkP2PTradeItemData : NetworkModel {
    public string playerName;
    public string itemJson;
    public NetworkP2PTradeItemData(string _playerName, string _itemJson) {
        playerName = _playerName;
        itemJson = _itemJson;
    }
}