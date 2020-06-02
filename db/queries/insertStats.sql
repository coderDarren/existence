/*insert into stats (
strength, 
dexterity, 
intelligence, 
fortitude, 
nanoPool,
nanoResist,
treatment,
firstAid,
oneHandEdged,
twoHandEdged,
pistol,
shotgun,
evades,
crit,
attackSpeed,
hacking,
engineering,
programming,
quantumMechanics,
symbiotics,
processing)
values (
	0, #strength
    1, #dexterity
    0, #intelligence
    0, #fortitude
    0, #nanoPool
    0, #nanoResist
    5, #treatment
    0, #firstAid
    0, #oneHandEdged
    0, #twoHandEdged
    0, #pistol
    0, #shotgun
    0, #evades
    1, #crit
    0, #attackSpeed
    0, #hacking
    0, #engineering
    0, #programming
    0, #quantumMechanics
    0, #symbiotics
    0 #processing
);*/

#delete from stats where id != 26;
select * from stats;