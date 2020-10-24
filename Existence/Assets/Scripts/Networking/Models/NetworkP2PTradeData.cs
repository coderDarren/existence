
public class NetworkP2PTradeData : NetworkModel {
    public string playersAccepted;
}

public class NetworkP2PTradeItemData : NetworkModel {
    public string playerName;
    public string itemJson;
    public NetworkP2PTradeItemData(string _playerName, string _itemJson) {
        playerName = _playerName;
        itemJson = _itemJson;
    }
}

public class NetworkP2PTradeTixData : NetworkModel {
    public string playerName;
    public int tix;
    public NetworkP2PTradeTixData(string _playerName, int _tix) {
        playerName = _playerName;
        tix = _tix;
    }
}