using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

/// Manages shooting behavior of Projectiles
public class PlayerShoot : PlayerShootBehavior {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // Only clients can send shoot requests for their own tank
        if (Input.GetKeyDown(KeyCode.Space) && GetComponentInParent<PlayerController>().networkObject.IsOwner && ServerInfo.playerNum > 0) {
            networkObject.SendRpc(RPC_SHOOT, Receivers.Server, ServerInfo.playerNum);
        }
    }

    // Server only RPC
    public override void Shoot(RpcArgs args) {
        if (ServerInfo.isServer) {
            int ownerNum = args.GetNext<int>();
            Projectile newProj = NetworkManager.Instance.InstantiateProjectile(0, transform.position, transform.rotation) as Projectile;
            // Set projectile data, to be used for NetworkStart
            newProj.tempOwnerNum = ownerNum;
            Physics.IgnoreCollision(GetComponentInParent<BoxCollider>(), newProj.GetComponent<BoxCollider>());
        } else {
            Debug.LogError("Server-only RPC Shoot was called on a client!");
        }
    }
}