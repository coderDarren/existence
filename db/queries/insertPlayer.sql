#insert into players (accountID, serverID, `name`)
#values (4, 1, "kyle");
#alter table players add column tix int not null default 0;
-- alter table sessionData modify column posX int not null default -86;
-- alter table sessionData modify column posY int not null default 0;
-- alter table sessionData modify column posZ int not null default 31;
-- alter table players modify column tix int not null default 10000;
select * from players;
select * from accounts;
select * from sessionData;