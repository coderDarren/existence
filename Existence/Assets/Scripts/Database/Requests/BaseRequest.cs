
public class BaseRequest : NetworkModel {
    public int id;
    public string apiKey;
    public BaseRequest(int _id, string _apiKey) {
        id = _id;
        apiKey = _apiKey;
    }
}