#insert into items (requirementsID, effectsID, level, shopBuyable, rarity, name, stackable, tradeskillable)
#values (44, 45, 1, true, 1, "Empty Canister", true, true);
#alter table items add column icon varchar(255) not null default '/db/icons/default.png';
select * from items