
public class CreatePlayerRequest : BaseRequest {
    public string name;
    public CreatePlayerRequest(string _name, int _id, string _apiKey) : base(_id, _apiKey) {
        name = _name;
    }
}