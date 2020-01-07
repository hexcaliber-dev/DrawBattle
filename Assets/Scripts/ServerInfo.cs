using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerInfo : ServerInfoBehavior {
    public const string REMOTE_SERVER_IP = "127.0.0.1";
    public const string LOCAL_SERVER_IP = "127.0.0.1";

    public const ushort SERVER_PORT = 15937;

    public static int playerNum = 0;

    public static bool isServer = false;

    public enum GamePhase {None, Drawing, Battling, Voting};

    public GamePhase currPhase = GamePhase.None;

    // Start is called before the first frame update
    void Start() {
        isServer = Application.isBatchMode;

        if (isServer) {
            // Reduce refresh rate
            Application.targetFrameRate = 30;

            print("==================================\n=    DrawBattle Server v1.0.0    =\n==================================");

            GameObject.FindObjectOfType<MainMenu>().Host();
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        print("Connected to Network");
        InitServer();
        NetworkManager.Instance.Networker.disconnected += delegate {Disconnect();};
    }

    // RPC Behavior
    public override void JoinGame(RpcArgs args) {
        networkObject.numPlayers++;
        Debug.Log("Player " + networkObject.numPlayers + " joined the game");
    }

    public override void ChangePhase(RpcArgs args) {
        currPhase = (GamePhase)(args.GetNext<int>());
        print("Changing to " + currPhase.ToString() + " phase");
        SceneManager.LoadScene((int)currPhase);
    }

    // Join button behavior
    public void RequestJoin() {
        playerNum = networkObject.numPlayers + 1;
        networkObject.SendRpc(RPC_JOIN_GAME, Receivers.Server);
    }

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
        SceneManager.LoadScene(0);
    }
}