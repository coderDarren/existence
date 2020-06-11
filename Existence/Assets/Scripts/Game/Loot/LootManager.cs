
using UnityEngine;
using UnityCore.Menu;

public class LootManager : GameSystem
{

    
    public NetworkEntityHandler entityHandler;
    public PageController pageManager;
    public LootPreviewPage previewPage;
    
#region Unity Functions
    private void Awake() {
    }

    private void Start() {       
        Mob.OnMobInit += OnMobInit;
        entityHandler.OnMobDidDie += OnMobDidDie;
        entityHandler.OnMobDidExit += OnMobDidExit;
    }

    private void OnDisable() {
        Mob.OnMobInit -= OnMobInit;
        entityHandler.OnMobDidDie -= OnMobDidDie;
        entityHandler.OnMobDidExit -= OnMobDidExit;
    }
#endregion

#region Public Functions

#endregion

#region Private Functions
    // track loot when a dead mob enters the scene
    private void OnMobInit(Mob _mob) {
        if (!_mob.data.dead) return;
        if (!MobHasLoot(_mob)) return;
        CheckLootPreviewIntegrity();
        previewPage.OnLootAdd(_mob);
    }

    // track freshly dead mobs
    private void OnMobDidDie(Mob _mob) {
        if (!MobHasLoot(_mob)) return;
        CheckLootPreviewIntegrity();
        previewPage.OnLootAdd(_mob);
    }

    // track out of range loot
    private void OnMobDidExit(Mob _mob) {
        if (!MobHasLoot(_mob)) return;
        CheckLootPreviewIntegrity();
        previewPage.OnLootRemove(_mob);
    }

    // call to ensure page is on
    // always turns on by default
    private void CheckLootPreviewIntegrity() {
        // !! TODO
        // Offer opt-out loot preview notifications in settings

        if (!pageManager.PageIsOn(PageType.LootPreview)) {
            pageManager.TurnPageOn(PageType.LootPreview);
        }
    }

    private bool MobHasLoot(Mob _mob) {
        if (_mob.data.lootPreview == null) return false;
        if (_mob.data.lootPreview.Length == 0) return false;
        return true;
    }
#endregion
}
