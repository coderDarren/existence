#insert into players (accountID, serverID, `name`)
#values (4, 1, "kyle");
#alter table players add column statPoints int not null default 0;
#update players set xp = 0, statPoints = 0, level = 1 where id = 17 or id = 18;
select * from players