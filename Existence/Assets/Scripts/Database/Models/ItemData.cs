
public class ItemData
{
    public string name;
    public StatData requirements;
    public StatData effects;
    public int level;
    public int rarity;
    public bool shopBuyable;
    public bool stackable;
    public bool tradeskillable;
    public string icon;

    // location in inventory. -1 by default
    public int slotLoc;
    public int slotID;
}
