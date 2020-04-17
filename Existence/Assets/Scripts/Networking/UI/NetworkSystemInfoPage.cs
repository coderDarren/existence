
using UnityEngine;
using UnityEngine.UI;

public class NetworkSystemInfoPage : GameSystem
{
    [Header("Pages")]
    public GameObject loadingPage;
    public GameObject cpuPage;
    public GameObject memPage;

    [Header("General")]
    public Text serverName;
    public Text serverVersion;
    public Text playersOnline;
    public GameObject navbar;

    [Header("CPU Labels")]
    public Text cpuManufacturer;
    public Text cpuBrand;
    public Text cpuSpeed;
    public Text cpuNumCores;
    public Text cpuNumProcessors;
    public Text cpuAvgLoad;
    public Text cpuCurrLoad;

    [Header("MEM Labels")]
    public Text memTotal;
    public Text memFree;
    public Text memUsed;
    public Text memActive;

    private NetworkController m_Network;
    private int m_ActivePage;
    private GameObject[] m_Pages;
    private bool m_Minimized=true;

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
        m_Pages = new GameObject[3]{loadingPage,cpuPage,memPage};
        Minimize();
        if (!network) return;
        network.OnInstanceUpdated += OnInstanceUpdated;
        network.OnConnect += OnServerConnect;
    }

    private void OnDisable() {
        if (!network) return;
        network.OnInstanceUpdated -= OnInstanceUpdated;
        network.OnConnect -= OnServerConnect;
    }
#endregion

#region Public Functions
    public void OpenCPUPage() {
        OpenPage(1);
    }

    public void OpenMEMPage() {
        OpenPage(2);
    }

    public void ToggleSize() {
        if (m_Minimized) {
            Maximize();
        } else {
            Minimize();
        }
    }
#endregion

#region Private Functions
    private void OnServerConnect() {
        m_ActivePage = 1;
        if (!m_Minimized) {
            OpenCPUPage();
        }
    }

    private void OnInstanceUpdated(NetworkInstanceData _instance) {
        var _sys = _instance.system;
        // General
        serverName.text = _sys.serverName;
        serverVersion.text = _sys.serverVersion;

        SetLabel("Players Online", playersOnline, _instance.gameInfo.playerCount.ToString());

        // CPU
        SetLabel("Manufacturer", cpuManufacturer, _sys.cpu.manufacturer);
        SetLabel("Brand", cpuBrand, _sys.cpu.brand);
        SetLabel("Speed", cpuSpeed, _sys.cpu.speed);
        SetLabel("# Cores", cpuNumCores, _sys.cpu.numCores);
        SetLabel("# Processors", cpuNumProcessors, _sys.cpu.numProcessors);
        SetLabel("Avg. Load", cpuAvgLoad, _sys.cpu.avgLoad);
        SetLabel("Current Load", cpuCurrLoad, _sys.cpu.currLoad);

        // MEM
        SetLabel("Total", memTotal, _sys.mem.total);
        SetLabel("Free", memFree, _sys.mem.free);
        SetLabel("Used", memUsed, _sys.mem.used);
        SetLabel("Active", memActive, _sys.mem.active);
    }

    private void SetLabel(string _name, Text _label, string _val) {
        _label.text = _name+": <color=#fc0>"+_val+"</color>";
    }

    
    private void Minimize() {
        navbar.SetActive(false);
        m_Minimized = true;
        ClosePages();
    }

    private void Maximize() {
        navbar.SetActive(true);
        m_Minimized = false;
        OpenPage(m_ActivePage);
    }

    private void ClosePages() {
        for (int i = 0; i < m_Pages.Length; i++) {
            m_Pages[i].SetActive(false);
        }
    }

    private void OpenPage(int _index) {
        if (_index < 0) return;
        if (_index > m_Pages.Length - 1) return;
        if (m_Pages[_index].activeSelf) return;

        for (int i = 0; i < m_Pages.Length; i++) {
            if (i == _index) {
                m_ActivePage = _index;
                m_Pages[i].SetActive(true);
            } else {
                m_Pages[i].SetActive(false);
            }
        }
    }
#endregion
}
