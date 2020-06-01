#insert into accounts (username, first_name, last_name, apiKey)
#values ('kyle123', 'kyle', 'oneale', 'a1b2c3key');
#alter table accounts add column email varchar(255) unique;
#SELECT * FROM existence.accounts;

select items.*, inventorySlots.ID as slotID, inventorySlots.loc as slotLoc from items
inner join inventorySlots on inventorySlots.playerID = 18 and inventorySlots.itemID = items.ID