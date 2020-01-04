using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class PlayerShoot : PlayerShootBehavior {

    public static byte[] textureData;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && GetComponentInParent<PlayerController>().networkObject.IsOwner)
            networkObject.SendRpc(RPC_SHOOT, Receivers.Server, textureData);
    }

    public override void Shoot(RpcArgs args) {
        byte[] texture = args.GetNext<byte[]>();
        Projectile newProj = NetworkManager.Instance.InstantiateProjectile(0, transform.position, transform.rotation) as Projectile;
        newProj.tempTextureData = texture;
    }
}