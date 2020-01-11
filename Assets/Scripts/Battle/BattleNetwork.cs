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

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    protected override void NetworkStart() {
        base.NetworkStart();

        // if(ServerInfo.isServer) {
        networkObject.SendRpc(RPC_SPAWN_TANK, Receivers.Server, ServerInfo.playerNum);
        // }
    }

    // Server only RPC to spawn tanks on join
    public override void SpawnTank(RpcArgs args) {
        int ownerNum = args.GetNext<int>();

        if(ownerNum > 0) {
            print("Spawn tank " + ownerNum);
            PlayerController newTank = NetworkManager.Instance.InstantiatePlayerController(0, spawnLocations[ownerNum - 1].position, Quaternion.identity) as PlayerController;
            newTank.playerNum = ownerNum;
            newTank.owner = args.Info.SendingPlayer;
            networkObject.SendRpc(args.Info.SendingPlayer, RPC_ASSIGN_MY_PLAYER);
        }
    }

    // Client only RPC to let the rest of the game know which tank is this client's
    public override void AssignMyPlayer(RpcArgs args) {
        foreach(PlayerController tank in GameObject.FindObjectsOfType<PlayerController>()) {
            if(tank.playerNum == ServerInfo.playerNum) {
                myPlayer = tank;
                Camera.main.GetComponent<CameraFollow>().player = myPlayer.transform;
                return;
            }
        }

        Debug.LogError("Tank " + ServerInfo.playerNum + " could not be found!!");
    }
}