using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class ServerInfo : ServerInfoBehavior {
    public static string SERVER_IP = "127.0.0.1";

    public static int playerNum = 0;

    public static bool isServer = false;

    // Start is called before the first frame update
    void Start() {
        isServer = Application.isBatchMode;
        DontDestroyOnLoad(gameObject);

        if (isServer) {
            // Reduce refresh rate
            Application.targetFrameRate = 30;

            print("==================================\n=    DrawBattle Server v1.0.0    =\n==================================");

            GetComponent<MultiplayerMenu>().Host();

            networkObject.numPlayers = 0;
        } else {
            Application.targetFrameRate = 120;
        }
    }

    // Update is called once per frame
    void Update() {
        print(playerNum);
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        print("Connected to Network");
        InitServer();
    }

    // RPC Behavior
    public override void JoinGame(RpcArgs args) {
        networkObject.numPlayers++;
        Debug.Log("Player " + networkObject.numPlayers + " joined the game");
    }

    // Join button behavior
    public void RequestJoin() {
        playerNum = networkObject.numPlayers + 1;
        networkObject.SendRpc(RPC_JOIN_GAME, Receivers.AllBuffered);
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
}