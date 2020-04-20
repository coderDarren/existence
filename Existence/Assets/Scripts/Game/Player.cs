
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hold data about the player
/// </summary>
public class Player : MonoBehaviour
{
    public Text nameLabel;
    public Session session;
    
    private PlayerData m_Data;

    public PlayerData data {
        get {
            return m_Data;
        }
    }

#region Unity Functions
    private void Awake() {
        // start with default name
        m_Data = new PlayerData();
        m_Data.player = new PlayerInfo();
        m_Data.stats = new StatData();
        m_Data.player.name = RandomString(12);
        nameLabel.text = m_Data.player.name;
    }
#endregion

#region Public Functions
    /// <summary>
    /// Call from session when network connects
    /// </summary>
    public void ConnectWithData(PlayerData _data) {
        m_Data = _data;
        nameLabel.text = _data.player.name;
    }
#endregion

#region Private Functions
    private string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0,chars.Length)];
        }

        return new string(stringChars);
    }
#endregion
}
