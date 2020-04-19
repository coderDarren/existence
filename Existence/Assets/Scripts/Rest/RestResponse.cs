
public class RestResponse 
{
	private ushort m_StatusCode;
	private string m_Message;

	public ushort statusCode {
		get {
			return m_StatusCode;
		}
	}

	public string message {
		get {
			return m_Message;
		}
	}

	public RestResponse(ushort _status, string _message) {
		m_StatusCode = _status;
		m_Message = _message;
	}
}
