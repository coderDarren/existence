using System.Collections.Generic;

public enum Sex {
    AGNOSTIC,
    MALE,
    FEMALE
}

public enum Race {
    AGNOSTIC,
    HUMAN
}

[System.Serializable]
public class PlayerInfo : NetworkModel {
    public int ID;
    public string name;
    public int level;
    public int xp;
    public int statPoints;
    public int health;
    public int maxHealth;
    public int tix;
    public Sex sex;
    public Race race;
}

public class PlayerSessionData : NetworkModel {
    public int ID;
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
}

public class PlayerEquipmentData : NetworkModel {
    public List<ArmorItemData> armor;
    public List<WeaponItemData> weapons;
}

public class PlayerData : NetworkModel {
    public PlayerInfo player;
    public PlayerSessionData sessionData;
    public StatData stats;
    // bring inventory down in string format. client is responsible for manually parsing subtypes into 'inventory'
    public string[] inventoryData;
    public string[] equipmentData;
    // this inventory list built manually by the player when connecting
    // check Player.InitializeInventory
    public List<IItem> inventory;
    public PlayerEquipmentData equipment;

    /* Helper property to provide insight into player creation failure
     * 200 - OK
     * 1400 - Username must be at least 5 characters
     * 1401 - Username cannot begin with a number or special character
     * 1402 - Password must be at least 8 characters
     * 1403 - Password cannot begin with a special character or a number
     */
    public int responseCode;

    public PlayerData(int _responseCode) {
        responseCode = _responseCode;
    }
    public PlayerData() {}
}
