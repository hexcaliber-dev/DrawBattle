using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Used to keep track of vital networking information and behaviors.
public class ServerInfo : ServerInfoBehavior {

    // TEMPORARY VALUE - replace with proper nat support etc for multiple server ips 
    public const string REMOTE_SERVER_IP = "bigrip.ocf.berkeley.edu";

    // Used for hosting, but NOT connecting. Connecting dynamically searches for local ip's
    public const string LOCAL_SERVER_IP = "127.0.0.1";

    // Used for LOCAL games, and temporarily for online-- but in the future online games will have dynamically assigned ports
    public const ushort SERVER_PORT = 15937;

    // This player's number, assigned by join order
    public static int playerNum = 0;

    // Is this instance a host? (Self-hosts are simultaneously servers and clients)
    public static bool isServer = false;

    // Used to keep track of game states. Each value corresponds to a scene index
    public enum GamePhase { None, Drawing, Battling, Voting }

    public GamePhase currPhase = GamePhase.None;

    // Start is called before the first frame update
    void Start() {
        isServer = Application.isBatchMode;

        // Prepare dedicated servers
        if (isServer) {
            // Reduce refresh rate to conserve cpu power
            Application.targetFrameRate = 30;
            print("=======================================\n=    DrawBattle Server v1.0.0-alpha   =\n=======================================");
            GameObject.FindObjectOfType<MainMenu>().Host();
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() { }

    // Is run when a connection is established
    protected override void NetworkStart() {
        base.NetworkStart();
        print("Connected to Network");
        InitServer();
        // Add custom disconnect behavior
        NetworkManager.Instance.Networker.disconnected += delegate { Disconnect(); };
    }

    // Server-only RPC
    public override void JoinGame(RpcArgs args) {
        networkObject.numPlayers++;
        Debug.Log("Player " + networkObject.numPlayers + " joined the game");
    }

    // All instance RPC
    public override void ChangePhase(RpcArgs args) {
        currPhase = (GamePhase) (args.GetNext<int>());
        print("Changing to " + currPhase.ToString() + " phase");
        SceneManager.LoadScene((int) currPhase);
    }

    // Join button behavior
    public void RequestJoin() {
        playerNum = networkObject.numPlayers + 1;
        networkObject.SendRpc(RPC_JOIN_GAME, Receivers.Server);
    }

    // Should be run on connection established
    void InitServer() {
        // Debug.Log(next.buildIndex);
        if (isServer) {
            networkObject.numPlayers = 0;
            print("Server Initialized");
        }

        if (!Application.isBatchMode) {
            RequestJoin();
        }
    }

    void Disconnect() {
        print("Disconnected from Server: Lost Connection");
        MainThreadManager.Run(() => {
            SceneManager.sceneLoaded += LoadLostConnectionPanel;
            SceneManager.LoadScene(0);
        });
    }

    // Opens the notification of a connection lost on the main menu
    void LoadLostConnectionPanel(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            CanvasGroup lostConnPanel = GameObject.Find("Lost Connection Panel").GetComponent<CanvasGroup>();
            lostConnPanel.alpha = 1;
            lostConnPanel.blocksRaycasts = true;
            SceneManager.sceneLoaded -= LoadLostConnectionPanel;
        }
    }
}