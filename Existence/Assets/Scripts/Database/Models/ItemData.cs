
public class ItemData : NetworkModel
{
    public int id;
    public string name;
    public string description;
    public StatData requirements;
    public StatData effects;
    public int level;
    public int rarity;
    public bool shopBuyable;
    public bool stackable;
    public bool tradeskillable;
    public string icon;

    // location in inventory. -1 by default
    public int slotLoc=-1;
    public int slotID;

    public ItemData(int _id) {
        id = _id;
    }
}
