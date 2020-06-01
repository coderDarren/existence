#insert into items (requirementsID, effectsID, level, shopBuyable, rarity, name, stackable, tradeskillable)
#values (44, 45, 1, true, 1, "Empty Canister", true, true);
#alter table items add column inventorySlotId int not null default -1;
select * from items