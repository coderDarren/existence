
public class AuthenticationData : NetworkModel
{
    public string username;
    public string password;
    public AuthenticationData(string _un, string _pass) {
        username = _un;
        password = _pass;
    }
}
