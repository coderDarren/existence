using System.Collections.Generic;

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

public class NetworkShopTerminalSellInfo : NetworkModel {
    public int itemID;
    public int inventoryLoc;
    public int price;
    public NetworkShopTerminalSellInfo(int _id, int _loc, int _pr){
        itemID = _id;
        inventoryLoc = _loc;
        price = _pr;
    }
}

public class NetworkShopTerminalBuyInfo : NetworkModel {
    public int itemID;
    public int level;
    public int price;
    public NetworkShopTerminalBuyInfo(int _id, int _lv, int _pr){
        itemID = _id;
        level = _lv;
        price = _pr;
    }
}

public class NetworkShopTerminalTradeData : NetworkModel {
    public string transactionId;
    public List<NetworkShopTerminalSellInfo> sell;
    public List<NetworkShopTerminalBuyInfo> buy;
    public NetworkShopTerminalTradeData(List<NetworkShopTerminalSellInfo> _s, List<NetworkShopTerminalBuyInfo> _b) {
        transactionId = "t-"+UnityEngine.Random.Range(1, 10000).ToString();
        sell = _s;
        buy = _b;
    }
}

public class NetworkShopTerminalTradeSuccessData : NetworkModel {
    public string transactionId;
    public int tix;
}