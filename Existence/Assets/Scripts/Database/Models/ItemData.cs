﻿public class PreviewItemData : NetworkModel {
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

public enum GearType {
    HEAD=0,
    CHEST=1,
    GLOVES=2,
    PANTS=3,
    BOOTS=4,
    NECK=5,
    BACK=6,
    L_SHOULDER=7,
    R_SHOULDER=8,
    L_SLEEVE=9,
    R_SLEEVE=10,
    L_HAND=11,
    R_HAND=12,
    LR_HAND=13
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
    public int slotType;
}

public class WeaponItemData : NetworkModel, IItem
{
    public ItemData def {get;set;}
    public int slotType;
    public int damageMin;
    public int damageMax;
    public int speed;

    public WeaponItemData(int _id, GearType _type, string _icon) {
        def = new ItemData(_id);
        def.itemType = ItemType.WEAPON;
        def.icon = _icon;
        slotType = (int)_type;
    }
}