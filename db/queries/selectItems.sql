#insert into items (requirementsID, effectsID, level, shopBuyable, rarity, name, stackable, tradeskillable)
#values (107, 108, 2, false, 1, "Low Amperage Phaser", false, false);
#alter table items add column description varchar(255) not null default "Nothing to see here.";
#update items set description = "" where ID = 5;
select * from items