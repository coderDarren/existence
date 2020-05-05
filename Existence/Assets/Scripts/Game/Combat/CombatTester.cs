
using UnityEngine;

public class CombatTester : GameSystem
{
    public KeyCode damageTestKey;
    

#region Unity Functions
    private void Update() {
        if (!GameObject.FindGameObjectWithTag("CombatTestDummy")) return;
        if (Input.GetKeyDown(damageTestKey)) {
            GameObject.FindGameObjectWithTag("CombatTestDummy").GetComponent<Mob>().Hit(50);
        }
    }
#endregion
}
