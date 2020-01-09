using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Most of this code was made by the Forge team. See https://github.com/BeardedManStudios/ForgeNetworkingRemastered/wiki for more info
public class MainMenu : MonoBehaviour {
    public bool DontChangeSceneOnConnect = false;
    public string masterServerHost = string.Empty;
    public ushort masterServerPort = 15940;
    public string natServerHost = string.Empty;
    public ushort natServerPort = 15941;

    public GameObject networkManager = null;
    public GameObject[] ToggledButtons;
    private NetworkManager mgr = null;
    private NetWorker server;

    private List<Button> _uiButtons = new List<Button>();
    private bool _matchmaking = false;
    public bool useMainThreadManagerForRPCs = true;
    public bool useInlineChat = false;

    public bool getLocalNetworkConnections = false;

    public bool useTCP = false;

    private void Start() {
        for (int i = 0; i < ToggledButtons.Length; ++i) {
            Button btn = ToggledButtons[i].GetComponent<Button>();
            if (btn != null)
                _uiButtons.Add(btn);
        }

        if (!useTCP) {
            // Do any firewall opening requests on the operating system
            NetWorker.PingForFirewall(ServerInfo.SERVER_PORT);
        }

        if (useMainThreadManagerForRPCs)
            Rpc.MainThreadRunner = MainThreadManager.Instance;

        // if (getLocalNetworkConnections) {
        //     NetWorker.localServerLocated += LocalServerLocated;
        //     NetWorker.RefreshLocalUdpListings(ServerInfo.SERVER_PORT);
        // }
        NetWorker.localServerLocated += LocalServerLocated;
    }

    private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender) {
        // Ignore virtual addresses from VMware
        if (!endpoint.Address.Contains("56")) {
            Debug.Log("Connecting to endpoint: " + endpoint.Address + ":" + endpoint.Port);

            MainThreadManager.Run(() => {
                Connect(endpoint.Address);
            });
        }
    }

    public void Connect(string ip) {

        NetWorker client;

        if (useTCP) {
            client = new TCPClient();
            ((TCPClient) client).Connect(ip, ServerInfo.SERVER_PORT);
        } else {
            client = new UDPClient();
            if (natServerHost.Trim().Length == 0) {
                ((UDPClient) client).Connect(ip, ServerInfo.SERVER_PORT);
            } else
                ((UDPClient) client).Connect(ip, ServerInfo.SERVER_PORT, natServerHost, natServerPort);
        }
        Connected(client);
    }

    public void Host() {
        if (useTCP) {
            server = new TCPServer(64);
            ((TCPServer) server).Connect();
        } else {
            server = new UDPServer(64);

            if (natServerHost.Trim().Length == 0)
                ((UDPServer) server).Connect(ServerInfo.LOCAL_SERVER_IP, ServerInfo.SERVER_PORT);
            else
                ((UDPServer) server).Connect(port: ServerInfo.SERVER_PORT, natHost: natServerHost, natPort: natServerPort);
        }

        server.playerTimeout += (player, sender) => {
            Debug.Log("Player " + player.NetworkId + " timed out");
        };
        //LobbyService.Instance.Initialize(server);

        Connected(server);

        ServerInfo.isServer = true;
        Debug.Log("Now hosting server on port " + ServerInfo.SERVER_PORT.ToString());
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.H))
        //     Host();
        // else if (Input.GetKeyDown(KeyCode.C))
        //     Connect();
        // else if (Input.GetKeyDown(KeyCode.L)) {
        //     NetWorker.localServerLocated -= TestLocalServerFind;
        //     NetWorker.localServerLocated += TestLocalServerFind;
        //     NetWorker.RefreshLocalUdpListings();
        // }
    }

    private void TestLocalServerFind(NetWorker.BroadcastEndpoints endpoint, NetWorker sender) {
        Debug.Log("Address: " + endpoint.Address + ", Port: " + endpoint.Port + ", Server? " + endpoint.IsServer);
    }

    public void Connected(NetWorker networker) {
        if (!networker.IsBound) {
            Debug.LogError("NetWorker failed to bind");
            return;
        }

        if (mgr == null && networkManager == null) {
            Debug.LogWarning("A network manager was not provided, generating a new one instead");
            networkManager = new GameObject("Network Manager");
            mgr = networkManager.AddComponent<NetworkManager>();
        } else if (mgr == null)
            mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

        // If we are using the master server we need to get the registration data
        JSONNode masterServerData = null;
        if (!string.IsNullOrEmpty(masterServerHost)) {
            string serverId = "myGame";
            string serverName = "Forge Game";
            string type = "Deathmatch";
            string mode = "Teams";
            string comment = "Demo comment...";

            masterServerData = mgr.MasterServerRegisterData(networker, serverId, serverName, type, mode, comment);
        }

        mgr.Initialize(networker, masterServerHost, masterServerPort, masterServerData);

        if (useInlineChat && networker.IsServer)
            SceneManager.sceneLoaded += CreateInlineChat;

        if (networker is IServer) {
            if (!DontChangeSceneOnConnect)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                NetworkObject.Flush(networker); //Called because we are already in the correct scene!
        }
    }

    public void RefreshLocal() {
        NetWorker.RefreshLocalUdpListings(ServerInfo.SERVER_PORT);
    }

    private void CreateInlineChat(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= CreateInlineChat;
        var chat = NetworkManager.Instance.InstantiateChatManager();
        DontDestroyOnLoad(chat.gameObject);
    }

    private void SetToggledButtons(bool value) {
        for (int i = 0; i < _uiButtons.Count; ++i)
            _uiButtons[i].interactable = value;
    }

    private void OnApplicationQuit() {
        if (getLocalNetworkConnections)
            NetWorker.EndSession();

        if (server != null) server.Disconnect(true);
    }
}