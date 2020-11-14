-- create table missionNodes (
-- 	# defaults
-- 	ID char(255) unique not null, # mish id,node id
--     nodeType int not null default 0,
--     title char(255) not null,
--     description char(255) not null,
--     progress float not null default 0.0,
--     rewards char(255) not null default "", # comma-delimited array of item ids
--     tixReward int not null default 0,
--     nextNodes char(255) not null default "", # comma-delimited array of node ids
--     
--     # sub properties
--     # FindItemsNode, ReturnItemsNode, CollectItemsNode
-- 	items char(255) not null default "", # comma-delimited array of item ids
--     # CollectItemsNode, KillMobsNode
--     count char(255) not null default "", # comma-delimited array of int counts respectively aligning with a task
--     # ReturnItemsNode, NPCChatNode
--     npc int not null default 0,
--     # FindNPCsNode
--     npcs char(255) not null default "", # comma-delimited array of npc ids
--     # KillMobsNode
--     mobs char(255) not null default "", # comma-delimited array of mob ids
--     # NPCChatNode
--     chatOptionOrder char(255) not null default "" # comma-delimited array of chat option indices
-- );
-- public enum MissionNodeType {
--     FIND_ITEMS, 0
--     RETURN_ITEMS, 1
--     COLLECT_ITEMS, 2
--     FIND_NPCS, 3
--     FIND_MOBS, 4
--     KILL_MOBS, 5
--     NPC_CHAT 6
-- }

/*
insert into missionNodes (
ID, nodeType, title, description, progress, rewards, tixReward, nextNodes,
items, count, npc, npcs, mobs, chatOptionOrder )
values (
	"killBots01,killBots01_20", # mish id,node id
    6, # node type
    "Hit Job", # title
    "Return to NPC.", # desc
    0, # progress
	"0", # rewards
    100, # tix reward
    "", # next nodes
    "", # items
    "", # count
    5, # npc
    "", # npcs
    "", # mobs
    "" # chatOptionOrder
);
*/

#update missionNodes set chatOptionOrder = "0" where ID = "killBots01,killBots01_20";

select * from missionNodes;