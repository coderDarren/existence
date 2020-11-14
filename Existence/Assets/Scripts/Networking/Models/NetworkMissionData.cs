using System.Collections.Generic;

public class MissionID : NetworkModel {
    public string mishID {get;}
    public string nodeID {get;}
    public MissionID(string _mishId, string _nodeId) {
        mishID = _mishId;
        nodeID = _nodeId;
    }
}

#region INTERFACES DEFS

public interface IMissionNode {
    string mishID {get;}
    string nodeID {get;}
    MissionNodeType type {get;set;}
    string[] rewardOptions {get;set;}
    int tixReward {get;set;}
    float progress {get;set;}
    string title {get;set;}
    string description {get;set;}
    string[] nextNodes {get;set;}
}

public interface IMissionInteraction {
    MissionID id {get;}
    string ToJsonString();
}

public interface IMissionResponse {
    MissionID id {get;set;}
    string message {get;set;}
    bool success {get;set;}
} 

#endregion

#region NODE DATA

public enum MissionNodeType {
    FIND_ITEMS,
    RETURN_ITEMS,
    COLLECT_ITEMS,
    FIND_NPCS,
    FIND_MOBS,
    KILL_MOBS,
    NPC_CHAT
}

public class MissionNode : NetworkModel, IMissionNode {
    public string mishID {get;}
    public string nodeID {get;}
    public MissionNodeType nodeType {get;set;}
    public string[] rewards {get;set;}
    public int tixReward {get;set;}
    public float progress {get;set;}
    public string title {get;set;}
    public string description {get;set;}
    public string[] nextNodes {get;set;}

    public static IMissionNode CreateNode(string _json) {
        string _searchStr = "\"nodeType\":";
        int _type = System.Int32.Parse(_json.Substring(_json.IndexOf(_searchStr)+_searchStr.Length, 1));
        
        switch ((MissionNodeType)_type) {
            case MissionNodeType.FIND_ITEMS: return NetworkModel.FromJsonStr<MissionFindItemsNode>(_json);
            case MissionNodeType.RETURN_ITEMS: return NetworkModel.FromJsonStr<MissionReturnItemsNode>(_json);
            case MissionNodeType.COLLECT_ITEMS: return NetworkModel.FromJsonStr<MissionCollectItemsNode>(_json);
            case MissionNodeType.FIND_NPCS: return NetworkModel.FromJsonStr<MissionFindNPCsNode>(_json);
            case MissionNodeType.FIND_MOBS: return NetworkModel.FromJsonStr<MissionFindMobsNode>(_json);
            case MissionNodeType.KILL_MOBS: return NetworkModel.FromJsonStr<MissionKillMobsNode>(_json);
            case MissionNodeType.NPC_CHAT: return NetworkModel.FromJsonStr<MissionNPCChatNode>(_json);
            default: return NetworkModel.FromJsonStr<MissionNode>(_json);
        }
    }
}

public class MissionFindItemsNode : MissionNode {
    public int[] items;
}

public class MissionReturnItemsNode : MissionNode {
    public int[] items;
    public int npc;
}

public class MissionCollectItemsNode : MissionNode {
    public int[] items;
    public int[] count;
}

public class MissionFindNPCsNode : MissionNode {
    public int[] npcs;
}

public class MissionFindMobsNode : MissionNode {
    public int[] mobs;
}

public class MissionKillMobsNode : MissionNode {
    public int[] mobs;
    public int[] count;
}

public class MissionNPCChatNode : MissionNode {
    public int[] chatOptionOrder;
    public int npc;
}

#endregion

#region INTERACTION DATA

public class MissionInteraction : NetworkModel, IMissionInteraction {
    public MissionID id {get;}
    public MissionInteraction(string _mishId, string _nodeId) {
        id = new MissionID(_mishId, _nodeId);
    }
}

public class MissionObjectInteraction : MissionInteraction {
    public string objectId;
    public MissionObjectInteraction(string _objId, string _mishId, string _nodeId) : base(_mishId, _nodeId) {
        objectId = _objId;
    }
}

public class MissionChatInteraction : MissionInteraction {
    public int chatOption;
    public MissionChatInteraction(int _option, string _mishId, string _nodeId) : base(_mishId, _nodeId) {
        chatOption = _option;
    }
}

public class MissionKillMobInteraction : MissionInteraction {
    public int mob;
    public MissionChatInteraction(int _mob, string _mishId, string _nodeId) : base(_mishId, _nodeId) {
        mob = _mob;
    }
}

#endregion

#region RESPONSE DATA

public class MissionResponse : NetworkModel, IMissionResponse {
    public MissionID id {get;set;}
    public string message {get;set;}
    public bool success {get;set;}
}

public class MissionObjectResponse : MissionResponse {
    public string objectId;
}

public class MissionChatResponse : MissionResponse {
    public int chatOption;
}

#endregion