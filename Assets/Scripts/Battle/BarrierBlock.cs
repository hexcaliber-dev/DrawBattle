using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

/// Used for network management during the battle phase
public class BarrierBlock : BarrierBlockBehavior {

    public int ownerNum;

    public int health = 100;

    MaterialPropertyBlock propBlock;
    Renderer rend;

    protected override void NetworkStart() {
        base.NetworkStart();
        if (ownerNum != 0) {
            ChangeColor(LobbyPlayer.PLAYER_COLOR_PRESETS[ownerNum - 1]);
        } else
            Debug.LogError("BarrierBlock owner was set to 0!");
    }

    void Awake() {
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();

    }

    public void DamageBlock(int damage) {
        // int damage = args.GetNext<int>();
        health -= damage;
        print("HEALTH: " + health);
        Color c = LobbyPlayer.PLAYER_COLOR_PRESETS[ownerNum - 1];
        ChangeColor(new Color(c.r, c.g, c.b, health / 100f));
        if (health <= 0 && networkObject.IsOwner) {
            networkObject.Destroy();
        }
    }

    // Change this block's color using PropertyBlocks. See http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/ for more info
    void ChangeColor(Color newColor) {
        print("CHANGE COLOR TO " + newColor);
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", newColor);
        rend.SetPropertyBlock(propBlock);
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Projectile" && col.gameObject.GetComponent<Projectile>().networkObject.ownerNum != ownerNum) {
            // networkObject.SendRpc(RPC_DAMAGE_BLOCK, Receivers.All, col.gameObject.GetComponent<Projectile>().damage);
            DamageBlock(col.gameObject.GetComponent<Projectile>().damage);
            col.gameObject.GetComponent<Projectile>().networkObject.Destroy();
        }
    }

}