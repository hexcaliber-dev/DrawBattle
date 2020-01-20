using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

/// Used for network management during the battle phase
public class BattleNetwork : BattleNetworkBehavior {

    public Transform[] spawnLocations;
    public static PlayerController myPlayer;

    public bool initialized = false;

    ServerInfo serverInfo;

    // Start is called before the first frame update
    void Start() {
        serverInfo = GameObject.FindObjectOfType<ServerInfo>();
    }

    // Update is called once per frame
    void Update() {
        if (!initialized && networkObject != null && ServerInfo.isServer && GameObject.FindObjectsOfType<PlayerController>().Length == serverInfo.networkObject.numPlayers) {
            networkObject.SendRpc(RPC_ASSIGN_MY_PLAYER, Receivers.All);
        }
    }

    protected override void NetworkStart() {
        base.NetworkStart();

        // Init homebase
        NetworkManager.Instance.InstantiateHomeBase(0, spawnLocations[ServerInfo.playerNum - 1].position, Quaternion.identity);

        // Spawn tank
        PlayerController newTank = NetworkManager.Instance.InstantiatePlayerController(0, spawnLocations[ServerInfo.playerNum - 1].position, Quaternion.identity) as PlayerController;
    }

    // Client only RPC to let the rest of the game know which tank is this client's
    public override void AssignMyPlayer(RpcArgs args) {
        foreach (PlayerController tank in GameObject.FindObjectsOfType<PlayerController>()) {
            if (tank.networkObject.playerNum == ServerInfo.playerNum) {
                myPlayer = tank;
                Camera.main.GetComponent<CameraFollow>().player = myPlayer.transform;
                GameObject.FindObjectOfType<Scoreboard>().InitScoreboard(myPlayer.GetComponent<PlayerStats>());
                initialized = true;
                return;
            }
        }

        Debug.LogError("Tank " + ServerInfo.playerNum + " could not be found!!");
    }
}