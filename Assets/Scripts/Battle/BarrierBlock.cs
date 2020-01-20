using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

/// Used for network management during the battle phase
public class BarrierBlock : BarrierBlockBehavior {

    public int ownerNum;

    // public int health = 100;

    MaterialPropertyBlock propBlock;
    Renderer rend;

    void Awake() {
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        if (networkObject.IsOwner) {
            networkObject.health = 99;
            networkObject.SendRpc(RPC_CHANGE_COLOR, Receivers.AllBuffered, 0);
        }
    }

    // Should be run by owner only
    public override void ChangeColor(RpcArgs args) {
        int damage = args.GetNext<int>();
        print("CHANGE COLOR " + ownerNum);
        if (networkObject != null) {
            Color c = ServerInfo.PLAYER_COLOR_PRESETS[ownerNum - 1];
            // Change this block's color using PropertyBlocks. See http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/ for more info
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", new Color(c.r, c.g, c.b, networkObject.health / 100f));
            rend.SetPropertyBlock(propBlock);
            if (networkObject.health < 0) {
                Destroy(gameObject);
            }
        }
    }

    void Update() { }
}