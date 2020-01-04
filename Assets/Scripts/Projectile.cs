using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class Projectile : ProjectileBehavior {

    Texture2D texture;

    public byte[] tempTextureData;

    public float speed = 1f;

    // Start is called before the first frame update
    void Start() {
        texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        if (tempTextureData != null) {
            texture.LoadRawTextureData(tempTextureData.Decompress());
            texture.Apply();
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }

    // Update is called once per frame
    void Update() {
        if (networkObject.IsOwner) {
            transform.position += transform.up * speed;
            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
        } else {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
        }
    }

    public override void ChangeTexture(RpcArgs args) {
        byte[] newTexture = args.GetNext<byte[]>().Decompress();
        texture.LoadRawTextureData(newTexture);
        print("Loaded new texture data");
        texture.Apply();
    }
}