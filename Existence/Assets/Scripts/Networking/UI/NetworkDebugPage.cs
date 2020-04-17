
using UnityEngine;
using UnityEngine.UI;

public class NetworkDebugPage : GameSystem
{
    public Toggle m_PredictiveSmoothingToggle;

    private NetworkController m_Network;

    private NetworkController network {
        get {
            if (!m_Network) {
                m_Network = NetworkController.instance;
            }
            if (!m_Network) {
                LogWarning("Trying to get network but no instance of NetworkController was found.");
            }
            return m_Network;
        }
    }

#region Unity Functions
    private void Start() {
        if (!network) return;
        m_PredictiveSmoothingToggle.isOn = network.usePredictiveSmoothing;
    }
#endregion

#region Public Functions
    public void TogglePredictiveSmoothing() {
        network.usePredictiveSmoothing = m_PredictiveSmoothingToggle.isOn;
    }
#endregion
}
