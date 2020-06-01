using System.Collections;
using System.Collections.Generic;

public enum ChatCommand
{
    UNKNOWN,
    LOGIN,
    LOGOUT,
    XP,
    INVENTORY
}

public class ChatCommandParser
{
    public ChatCommand ParseCommand(string _in, out List<string> _args) {
        string _cmdString = _in.Substring(1);
        string[] _argsArray = _cmdString.Split(' ');
        _args = new List<string>(_argsArray);

        switch (_args[0].ToLower()) {
            case "login": return ChatCommand.LOGIN;
            case "logout": return ChatCommand.LOGOUT;
            case "xp": return ChatCommand.XP;
            case "inventory": return ChatCommand.INVENTORY;
            default: return ChatCommand.UNKNOWN;
        }
    }
}
