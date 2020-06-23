#insert into items (requirementsID, effectsID, level, shopBuyable, rarity, name, stackable, tradeskillable)
#values (107, 108, 2, false, 1, "Low Amperage Phaser", false, false);
#alter table items add column description varchar(255) not null default "Nothing to see here.";
#update items set description = "" where ID = 5;
/*create table weaponItems(
	itemID int unique not null,
    weaponType int not null default 0,
    minDamage int not null default 0,
    maxDamage int not null default 0,
    speed int not null default 1,
    foreign key (itemID) references items(ID)
);*/
#alter table weaponItems add column speed int not null default 1;
#alter table items add column itemType int not null default 0;
#delete from items where ID >= 13;
#delete from stats where ID >= 124;
#delete from armorItems where ID >= 0;
select * from weaponItems;
select * from armorItems;
select * from items