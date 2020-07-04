select * from inventorySlots;
select * from equipmentSlots;

/*
# selects all weapons player has equipped
select items.*,equipmentSlots.ID as equipmentID,weaponItems.slotType from items 
inner join equipmentSlots on equipmentSlots.playerID = 18 and items.ID = equipmentSlots.itemID
inner join weaponItems on items.ID = weaponItems.itemID and weaponItems.slotType = 2;
*/

# selects all inventory belonging to a player
#select * from items inner join inventorySlots on inventorySlots.playerID = 18 and items.ID = inventorySlots.itemID;
# selects all equipment belonging to a player
select * from items inner join equipmentSlots on equipmentSlots.playerID = 20 and items.ID = equipmentSlots.itemID;