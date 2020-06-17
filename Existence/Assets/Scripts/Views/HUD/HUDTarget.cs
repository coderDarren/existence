
using UnityEngine;
using UnityEngine.UI;

public class HUDTarget : MonoBehaviour
{
    public Text name;
    public Text lvl;
    public Text hpLabel;
    public Image hp;
    public Image energy;
    
#region Public Functions
    public void Init(NameplateData _data) {
        UpdateShadowedText(name, _data.name);
        UpdateShadowedText(lvl, "LV. " + _data.lvl);
        hp.fillAmount = _data.health / (float)_data.maxHealth;
        hpLabel.text = _data.health + "/" + _data.maxHealth;
    }

    public void UpdateData(NameplateData _data) {
        hp.fillAmount = _data.health / (float)_data.maxHealth;
        hpLabel.text = _data.health + "/" + _data.maxHealth;
    }
#endregion

#region Private Functions
    private void UpdateShadowedText(Text _t, string _content) {
        Text[] _texts = _t.GetComponentsInChildren<Text>();
        foreach (Text _text in _texts) {
            _text.text = _content;
        }
    }
#endregion
}
