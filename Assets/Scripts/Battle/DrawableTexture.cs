using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

/// Attach onto any object that can be 
public class DrawableTexture : MonoBehaviour {
    // 3D array: 1st is player number, 2nd is drawing type, 3rd is bytearray of raw texture data.
    // Initialized in ServerInfo
    public static byte[][][] textures;

    // Assign in inspector
    public PlayerDraw.Drawings drawingType;

    private Texture2D texture;

    // Remember to call this on any accompanying scripts once owner has been assigned!
    public void ChangeTexture(int owner) {
        byte[] data = textures[owner][(int) drawingType];
        if (data != null && data.Length > 0) {
            texture.LoadRawTextureData(textures[owner][(int) drawingType]);
            texture.Apply();
        }
    }

    void Start() {
        texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}