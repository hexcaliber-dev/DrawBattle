using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class BattleNetwork : BattleNetworkBehavior {

    public Transform[] spawnLocations;

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

    public override void SpawnTank(RpcArgs args) {
        int ownerNum = args.GetNext<int>();
        print("Spawn tank " + ownerNum);
        PlayerController newTank = NetworkManager.Instance.InstantiatePlayerController(0, spawnLocations[ownerNum - 1].position, Quaternion.identity) as PlayerController;
        newTank.owner = args.Info.SendingPlayer;
    }
}