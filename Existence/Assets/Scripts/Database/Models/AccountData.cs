
public class AccountData : NetworkModel
{
    public int id;
    public string first_name;
    public string last_name;
    public string apiKey;
    public string username;
    public string email;
    public string password;

    /* Helper property to provide insight into account creation failure
     * 200 - OK
     * 1400 - Username must be at least 5 characters
     * 1401 - Username cannot begin with a number or special character
     * 1402 - Password must be at least 8 characters
     * 1403 - Password cannot begin with a special character or a number
     * 1404 - First name cannot contain special characters or numbers
     * 1405 - First name cannot contain special characters or numbers
     * 1406 - Email is not supported
     * 1407 - Account already exists for username
     * 1408 - Account already exists for email
     */
    public int creationResponseCode;

    public AccountData(int _creationResponseCode) {
        creationResponseCode = _creationResponseCode;
    }

    public AccountData() {}
}
