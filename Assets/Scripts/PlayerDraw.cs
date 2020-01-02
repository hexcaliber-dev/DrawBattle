using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerDraw : PlayerDrawBehavior {

    public PaintCanvas paintCanvas;

    private void Start() {
        var data = paintCanvas.GetAllTextureData();
        var zippeddata = data.Compress();

        // RpcSendFullTexture(zippeddata);
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                var pallet = hit.collider.GetComponent<PaintCanvas>();
                if (pallet != null) {
                    Debug.Log(hit.textureCoord);
                    Debug.Log(hit.point);

                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    MeshCollider meshCollider = hit.collider as MeshCollider;

                    if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                        return;

                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    // CmdBrushAreaWithColorOnServer(pixelUV, ColorPicker.SelectedColor, BrushSizeSlider.BrushSize);
                    // BrushAreaWithColor(pixelUV, ColorPicker.SelectedColor, BrushSizeSlider.BrushSize);
                    BrushAreaWithColor(pixelUV, Color.black, 5);
                }
            }
        }
    }

    // [Command]
    // private void CmdBrushAreaWithColorOnServer(Vector2 pixelUV, Color color, int size) {
    //     RpcBrushAreaWithColorOnClients(pixelUV, color, size);
    //     BrushAreaWithColor(pixelUV, color, size);
    // }

    // [ClientRpc]
    // private void RpcBrushAreaWithColorOnClients(Vector2 pixelUV, Color color, int size) {
    //     BrushAreaWithColor(pixelUV, color, size);
    // }

    public override void SendFullTexture(RpcArgs args) {
        byte[] textureData = args.GetNext<byte[]>();
        paintCanvas.SetAllTextureData(textureData.Decompress());
    }

    private void BrushAreaWithColor(Vector2 pixelUV, Color color, int size) {
        for (int x = -size; x < size; x++) {
            for (int y = -size; y < size; y++) {
                paintCanvas.Texture.SetPixel((int) pixelUV.x + x, (int) pixelUV.y + y, color);
            }
        }

        paintCanvas.Texture.Apply();
    }
}