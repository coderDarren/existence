create table mobs (
	ID int auto_increment not null unique,
    name varchar(255) unique,
    primary key (ID)
);

create table mobLootItems (
	ID int auto_increment not null unique,
    mobID int not null,
    itemID int not null,
    dropRate float not null default 0.1,
    primary key (ID),
    foreign key (mobID) references mobs(ID),
    foreign key (itemID) references items(ID)
);

select * from mobs;
select * from mobLootItems;