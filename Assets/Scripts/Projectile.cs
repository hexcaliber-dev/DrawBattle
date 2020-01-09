using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

// The stuff that gets shot from tanks. Instantiation is controlled by PlayerShoot
public class Projectile : ProjectileBehavior {

    public Texture2D texture;

    public byte[] tempTextureData;

    public float speed = 1f;
    public int tempOwnerNum = 0;

    bool initialized = false;

    // Start is called before the first frame update
    void Start() {
        texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }

    // Update is called once per frame
    void Update() {
        if(networkObject != null) {
            if (networkObject.IsOwner) {
                transform.position += transform.up * speed;
                networkObject.position = transform.position;
                networkObject.rotation = transform.rotation;
            } else {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
            }
        }

        // Check for 2 conditions: temp values were assigned properly AND object is connected to network
        if(!initialized && networkObject != null && tempOwnerNum > 0 && tempTextureData.Length > 0) {
            networkObject.ownerNum = tempOwnerNum;
            networkObject.SendRpc(RPC_INIT_PROJECTILE, Receivers.All, tempTextureData);
        }
    }

    // Run on InitProjectile to actually apply the given texture
    public void ApplyTexture(byte[] textureData) {
        // If not yet connected, save texturedata to load on NetworkStart
        if (networkObject == null) {
            tempTextureData = textureData;
        } else {
            if (textureData.Length > 0) {
                texture.LoadRawTextureData(textureData.Decompress());
                texture.Apply();
            } else {
                Debug.LogError("No texture data was saved to Projectile for player " + networkObject.ownerNum);
            }
        }
    }

    // All-instance RPC. Initializes textures for this projectile
    public override void InitProjectile(RpcArgs args) {
        byte[] textureData = args.GetNext<byte[]>();

        ApplyTexture(textureData);
        initialized = true;
    }
}