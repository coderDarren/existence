public class NameplateData {
    public string name;
    public int health;
    public int maxHealth;
    public bool displayHealth;
    public bool isVisible;  
    public NameplateData() {}
    public NameplateData(string _name, int _health, int _maxHealth, bool _displayHealth=false) {
        name = _name;
        health = _health;
        maxHealth = _maxHealth;
        displayHealth = _displayHealth;
    }
}