using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Data Listing", menuName = "Equipment/Data Listing", order = 1)]
public class EquipmentData : ScriptableObject
{
    [System.Serializable]
    public class Props {
        public Sex sex;
        public Race race;
        public GearType location; 
        public GameObject prefab;
        public Texture2D tex;
        public Utilities.RectBounds bounds;     
    }

    [System.Serializable]
    public class ListingObject {
        public EquipmentItem item;
        public Props props;
    }

    public ListingObject[] data;

    public enum EquipmentItem {
        LOW_AMPERAGE_PHASER=5,
        NEWCOMERS_LOW_AMPERAGE_PHASER=11,
        NEWCOMERS_TATTERED_VEST=14,
        NEWCOMERS_JAGGED_BLADE=15,
        NEWCOMERS_SEMIFUNCTIONAL_SHOTGUN=16,
        NEWCOMERS_TATTERED_CLOAK=17,
        NEWCOMERS_FUNCTIONAL_SHOTGUN=18,
        NEWCOMERS_SHARPENED_BLADE=19,
        BASIC_LEFT_ARM_ARTILLERY=20,
        BASIC_LEFT_ARM_FORCE=21,
        CHEAP_LEATHER_VEST=26,
        CHEAP_LEATHER_PANTS=27,
        CHEAP_LEATHER_SLEEVES=28
    }
}
