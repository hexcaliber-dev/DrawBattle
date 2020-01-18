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

    // Set to true when player quits on purpose
    public static bool expectingQuit = false;

    // Indices correspond with player numbers
    public static readonly Color[] PLAYER_COLOR_PRESETS = { Color.red, Color.yellow, Color.green, Color.blue };

    // Used to keep track of game states. Each value corresponds to a scene index
    public enum GamePhase { MainMenu, Lobbying, Drawing, Battling, Voting }

    // Current game phase. Use ChangePhase() to edit it.
    public static GamePhase currPhase = GamePhase.MainMenu;

    // Custom pen cursor
    public Texture2D cursorTexture;

    LobbyDots lobbyDots;

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

        SceneManager.sceneLoaded += ManageCursorOnSceneLoad;
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

    // All instance RPC
    public override void LeaveGame(RpcArgs args) {
        int leavingPlayer = args.GetNext<int>();
        if (isServer) {
            print("Player " + leavingPlayer + " left the game");
            networkObject.numPlayers--;
            lobbyDots.networkObject.SendRpc(LobbyDotsBehavior.RPC_REMOVE_DOT, Receivers.AllBuffered, leavingPlayer);
            if (networkObject.numPlayers == 0) {
                if (leavingPlayer == playerNum) {
                    print("Closing server...");
                } else {
                    print("All players have left! Restarting server...");
                    ChangePhase(GamePhase.Lobbying);
                }
            }
        }

        if (playerNum > leavingPlayer) {
            playerNum--;
        }
    }

    // Server-only RPC
    public override void JoinGame(RpcArgs args) {
        networkObject.numPlayers++;
        Debug.Log("Player " + networkObject.numPlayers + " joined the game");
        lobbyDots.networkObject.SendRpc(LobbyDotsBehavior.RPC_UPDATE_DOT, Receivers.AllBuffered, networkObject.numPlayers - 1, 1);
    }

    // Run on server only. This will update for all clients
    public static void ChangePhase(GamePhase newPhase) {
        currPhase = newPhase;
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
        if (expectingQuit) {
            expectingQuit = false;
        } else {
            print("Disconnected from Server: Lost Connection");
            MainThreadManager.Run(() => {
                SceneManager.sceneLoaded += LoadLostConnectionPanel;
                SceneManager.LoadScene(0);
            });
        }
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

    void ManageCursorOnSceneLoad(Scene scene, LoadSceneMode mode) {

        // Snuck in a little lobby dots management here lol
        if (scene.buildIndex == (int) GamePhase.Lobbying) {
            lobbyDots = GameObject.FindObjectOfType<LobbyDots>();
        }

        if (scene.buildIndex == (int) GamePhase.Drawing || scene.buildIndex == (int) GamePhase.Lobbying) {
            Cursor.SetCursor(cursorTexture, Vector2.up * 10f, CursorMode.Auto);
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}