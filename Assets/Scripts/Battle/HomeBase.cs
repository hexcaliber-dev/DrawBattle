using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class HomeBase : HomeBaseBehavior {
    protected override void NetworkStart() {
        base.NetworkStart();
        if (networkObject.IsOwner) {
            networkObject.SendRpc(RPC_INIT_BLOCKS, Receivers.AllBuffered, ServerInfo.playerNum);
        }
        // Reset z position
        transform.Translate(0, 0, -transform.position.z);
    }

    public override void InitBlocks(RpcArgs args) {
        int owner = args.GetNext<int>();
        foreach (BarrierBlock block in GetComponentsInChildren<BarrierBlock>()) {
            block.ownerNum = owner;
            block.ResetColor();
        }
    }
}