#alter table inventorySlots add column ID int not null primary key auto_increment;
insert into inventorySlots (playerID, itemID) values (18, 4);
select * from inventorySlots