using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

/// Attach onto any object that can be 
public class DrawableTexture : DrawableTextureBehavior {
    // An array of byte arrays. Each one corresponds to a texture for a different item (see PlayerDraw.drawingNames for readable descriptions)
    public static byte[][] textures = new byte[PlayerDraw.drawingNames.Length][];
    private byte[] textureData = new byte[0];
    private Texture2D texture;
    public int ownerNum = 0;
    bool initialized = false;

    // Assign in inspector
    public PlayerDraw.Drawings drawingType;

    public void InitDrawableTexture(byte[] tex, int owner) {
        textureData = tex;
        ownerNum = owner;
        texture.LoadRawTextureData(textureData.Decompress());
        texture.Apply();
    }

    void Start() {
        if (textures[(int) drawingType] != null) {
            texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
            GetComponent<Renderer>().material.mainTexture = texture;
            InitDrawableTexture(textures[(int)drawingType], ServerInfo.playerNum);
        }
    }

    void Update() {
        // If these conditions are all true, this is the owner of the texture (which holds all the data) and can send it to everyone else.
        if (!initialized && networkObject != null && ownerNum != 0 && textureData.Length > 0) {
            networkObject.SendRpc(RPC_SEND_TEXTURE, Receivers.All, textureData, ownerNum);
            initialized = true;
        }
    }

    public override void SendTexture(RpcArgs args) {
        InitDrawableTexture(args.GetNext<byte[]>(), args.GetNext<int>());
    }
}