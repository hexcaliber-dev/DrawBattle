using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

// The stuff that gets shot from tanks. Instantiation is controlled by PlayerShoot
public class Projectile : ProjectileBehavior {

    public float speed = 1f;
    public int damage = 10;
    public int tempOwnerNum = 0;

    bool initialized = false;

    void Start() { }

    protected override void NetworkStart() {
        base.NetworkStart();
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        if (tempOwnerNum != 0)
            networkObject.ownerNum = tempOwnerNum;
    }

    // Update is called once per frame
    void Update() {
        if (networkObject != null) {
            if (networkObject.IsOwner) {
                transform.position += transform.up * speed;
                networkObject.position = transform.position;
                networkObject.rotation = transform.rotation;
            } else {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "HomeBase") {
            if (ServerInfo.playerNum != col.gameObject.GetComponentInChildren<BarrierBlock>().ownerNum) {
                PlayerStats.getPlayerStatsFromNumber(col.gameObject.GetComponentInChildren<BarrierBlock>().ownerNum).ChangeStat("baseHealth", -damage);
            }
        }
    }
}