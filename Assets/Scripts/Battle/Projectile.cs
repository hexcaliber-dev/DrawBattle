using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

// The stuff that gets shot from tanks. Instantiation is controlled by PlayerController
public class Projectile : ProjectileBehavior {

    public float speed = 1f;
    public int damage = 10;
    public int tempOwnerNum = 0;

    bool initialized = false;

    void Start() { }

    protected override void NetworkStart() {
        base.NetworkStart();
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        if (tempOwnerNum != 0) {
            networkObject.ownerNum = tempOwnerNum;
        }

        if (!networkObject.IsOwner) {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (networkObject != null) {
            if (networkObject.IsOwner) {
                transform.position += transform.up * speed;
                networkObject.position = transform.position;
                networkObject.rotation = transform.rotation;
            } else if (networkObject.position.x != 0 && networkObject.position.y != 0) {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
            }

            if (!initialized && networkObject.ownerNum != 0) {
                GetComponent<DrawableTexture>().ChangeTexture(networkObject.ownerNum);
                initialized = true;
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if (networkObject.IsOwner) {
            if (col.gameObject.tag == "HomeBase") {
                int baseNum = col.gameObject.GetComponentInChildren<BarrierBlock>().ownerNum;
                if (ServerInfo.playerNum != baseNum) {
                    PlayerStats.getPlayerStatsFromNumber(baseNum).ChangeStat("baseHealth", -damage);
                }
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (networkObject != null) {
            // Collisions are ignored between player and their own base already 
            if (networkObject.IsOwner && col.gameObject.tag == "Barrier") {
                BarrierBlock block = col.gameObject.GetComponent<BarrierBlock>();
                if(!block.networkObject.IsOwner) {
                    block.networkObject.health -= damage;
                    block.networkObject.SendRpc(BarrierBlockBehavior.RPC_CHANGE_COLOR, Receivers.AllBuffered, -damage);
                    networkObject.Destroy();
                }
            }
        }
    }
}