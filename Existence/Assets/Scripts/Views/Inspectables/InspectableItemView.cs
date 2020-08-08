
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InspectableItemView : MonoBehaviour
{

    public Text name;
    public Text lvl;
    public Text rarity;
    public Text description;
    public Text requirements;
    public Text effects;
    public Image icon;

    private CanvasGroup m_Canvas;

#region Unity Functions
    private void Awake() {
        m_Canvas = GetComponent<CanvasGroup>();
    }
#endregion

#region Public Functions
    public void Open(ItemData _item) {
        name.text = _item.name;
        lvl.text = "LV. "+_item.level;
        description.text = _item.description;
        rarity.text = "Worthless";
        requirements.text = ConstructStatPreview(_item.requirements, false, "81E3FF");
        effects.text = ConstructStatPreview(_item.effects, true, "0f0");
        icon.sprite = Utilities.LoadStreamingAssetsSprite(_item.icon);
        m_Canvas.alpha = 1;
    }

    public void Close() {
        m_Canvas.alpha = 0;
    }
#endregion


#region Private Funtions
    private string ConstructStatPreview(StatData _stat, bool _showSign, string _statColor) {
        string _ret = string.Empty;
        int _count = 0;
        AddRequirementDescription("Strength", _stat.strength, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Dexterity", _stat.dexterity, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Intelligence", _stat.intelligence, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Fortitude", _stat.fortitude, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Nano Pool", _stat.nanoPool, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Nano Resist", _stat.nanoResist, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Treatment", _stat.treatment, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("First Aid", _stat.firstAid, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("1 Hand Edged", _stat.oneHandEdged, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("2 Hand Edged", _stat.twoHandEdged, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Pistol", _stat.pistol, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Shotgun", _stat.shotgun, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Evades", _stat.evades, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Critical Chance", _stat.crit, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Attack Speed", _stat.attackSpeed, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Hacking", _stat.hacking, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Engineering", _stat.engineering, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Programming", _stat.programming, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Quantum Mechanics", _stat.quantumMechanics, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Symbiotics", _stat.symbiotics, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Processing", _stat.processing, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Run Speed", _stat.runSpeed, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Melee", _stat.melee, ref _ret, ref _count, _showSign, _statColor);
        AddRequirementDescription("Heal Delta", _stat.hot, ref _ret, ref _count, _showSign, _statColor);
        _ret += "<color=#777>...</color>";
        return _ret;
    }

    private void AddRequirementDescription(string _name, int _stat, ref string _description, ref int _counter, bool _showSign, string _statColor) {
        if (_counter < 3 && _stat != 0) {
            _description += _name+": <color=#"+_statColor+">"+(_showSign ? _stat < 0 ? "-" : "+" : "")+_stat+"</color>\n";
            _counter++;
        }
    }
#endregion
}
