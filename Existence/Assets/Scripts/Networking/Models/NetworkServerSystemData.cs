
/// <summary>
/// Info about the server's hardware systems
/// </summary>
public class NetworkServerSystemData {
    public string serverName;
    public string serverVersion;
    public NetworkServerSystemCPUData cpu;
    public NetworkServerSystemMEMData mem;
}

public class NetworkServerSystemCPUData {
    public string manufacturer;
    public string brand;
    public string speed;
    public string numCores;
    public string numProcessors;
    public string avgLoad;
    public string currLoad;
}

public class NetworkServerSystemMEMData {
    public string total;
    public string free;
    public string used;
    public string active;
}