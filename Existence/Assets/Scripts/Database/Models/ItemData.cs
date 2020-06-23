public class PreviewItemData : NetworkModel {
    public int id;
    public int level;
    public string name;
    public string icon;
}

public enum ItemType {
    BASIC,
    WEAPON,
    ARMOR
}

public enum WeaponType {
    L_HAND,
    R_HAND,
    LR_HAND
}

public enum ArmorType {
    HEAD,
    CHEST,
    GLOVES,
    PANTS,
    BOOTS,
    NECK,
    BACK,
    L_SHOULDER,
    R_SHOULDER,
    L_SLEEVE,
    R_SLEEVE    
}

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
    public ItemType itemType;

    // location in inventory. -1 by default
    public int slotLoc=-1;
    public int slotID;

    public ItemData() {}
    public ItemData(int _id) {
        id = _id;
    }

    /*
     * CreateItem handles subtype parsing for all items in the game
     * Use this function on the item json to get the correct item data
     */
    public static IItem CreateItem(string _json) {
        if (_json.Contains("armorType")) {
            return NetworkModel.FromJsonStr<ArmorItemData>(_json);
        } else if (_json.Contains("weaponType")) {
            return NetworkModel.FromJsonStr<WeaponItemData>(_json);
        }

        return NetworkModel.FromJsonStr<BasicItemData>(_json);
    }
}

public interface IItem {
    ItemData def {get;set;}
}

public class BasicItemData : NetworkModel, IItem 
{
    public ItemData def {get;set;}
}

public class ArmorItemData : NetworkModel, IItem
{
    public ItemData def {get;set;}
    public int armorType;
}

public class WeaponItemData : NetworkModel, IItem
{
    public ItemData def {get;set;}
    public int weaponType;
    public int damageMin;
    public int damageMax;
    public int speed;
}