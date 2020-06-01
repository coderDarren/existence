
public class NetworkHandshake : NetworkModel
{
    
    public NetworkPlayerData player;
    public AccountData account;
    public int sessionId;

    public NetworkHandshake(NetworkPlayerData _player, AccountData _account, int _sessionId) {
        player = _player;
        account = _account;
        sessionId = _sessionId;
    }
}
