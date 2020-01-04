using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    public static byte[] textureData;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && GetComponentInParent<PlayerController>().networkObject.IsOwner)
            Shoot();
    }

    void Shoot() {
        Projectile newProj = NetworkManager.Instance.InstantiateProjectile(0, transform.position, transform.rotation) as Projectile;
        newProj.tempTextureData = textureData;
    }
}