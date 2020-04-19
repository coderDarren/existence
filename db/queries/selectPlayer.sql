select * from players
inner join playerSkills on players.ID = 5 and playerSkills.playerID = players.ID
inner join stats on playerSkills.statsID = stats.ID;