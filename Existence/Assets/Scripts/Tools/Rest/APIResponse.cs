using Newtonsoft.Json;

public class APIResponse 
{
	public string message;
	public int statusCode;

	public APIResponse(int _statusCode, string _message) {
		statusCode = _statusCode;
		message = _message;
	}

	public static APIResponse FromJson(string _json) {
		try {
			return JsonConvert.DeserializeObject<APIResponse>(_json);
		} catch (System.Exception) {
			return new APIResponse(500, "Failed to parse response.");
		}
	}
}
