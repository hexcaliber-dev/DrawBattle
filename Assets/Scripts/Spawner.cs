using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class Spawner : SpawnerBehavior {

    public List<Transform> spawnLocs;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() { }

    public override void SpawnObject(RpcArgs args) {
        int player = args.GetNext<int>();
        //    GameObject.Instantiate(spawnObjs[player], spawnLocs[player].position, Quaternion.identity);
        print("Spawn " + player);
        NetworkManager.Instance.InstantiatePlayerObj(player, spawnLocs[player].position, Quaternion.identity);
    }

    public void RequestSpawn() {
        networkObject.SendRpc(RPC_SPAWN_OBJECT, Receivers.AllBuffered, ServerInfo.playerNum - 1);
    }
}