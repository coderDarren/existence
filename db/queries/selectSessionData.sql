#insert into sessionData (posX, posY, posZ, rotX, rotY, rotZ) values (-21,33,275,0,0,0);
alter table sessionData modify column posX int not null default -21;
alter table sessionData modify column posY int not null default 33;
alter table sessionData modify column posZ int not null default 275;

alter table sessionData modify column rotX int not null default 0;
alter table sessionData modify column rotY int not null default 0;
alter table sessionData modify column rotZ int not null default 0;

update sessionData set posX = -21 where id >= 14;
update sessionData set posY = 33 where id >= 14;
update sessionData set posZ = 275 where id >= 14;
select * from sessionData;