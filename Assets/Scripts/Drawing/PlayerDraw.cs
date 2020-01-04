using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class PlayerDraw : PlayerDrawBehavior {

    public PaintCanvas paintCanvas;

    /// Last frame's mouse position. Used for lerping
    Vector2 prevPos;

    /// Used to keep track of mouseup/down events so we don't fill in the space between 2 points if there was a pen lift
    bool isDragging = false;

    private void Start() { }

    private void Update() {

        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                var pallet = hit.collider.GetComponent<PaintCanvas>();
                if (pallet != null) {

                    // Check to make sure the canvas was clicked
                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    MeshCollider meshCollider = hit.collider as MeshCollider;

                    if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                        return;

                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    Vector2 pixelUV = hit.textureCoord;

                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    if (!isDragging)
                        prevPos = pixelUV;
                    else
                        paintCanvas.ColorBetween(prevPos, pixelUV, Color.black, 5);

                    paintCanvas.BrushAreaWithColor(pixelUV, Color.black, 5);
                    prevPos = pixelUV;
                    isDragging = true;
                }
            }
        }
    }

    public override void SendFullTexture(RpcArgs args) {
        byte[] textureData = args.GetNext<byte[]>();
        paintCanvas.SetAllTextureData(textureData.Compress());

        // // Testing
        // NetworkManager.Instance.InstantiateProjectile().networkObject.SendRpc(RPC_SEND_FULL_TEXTURE, Receivers.AllBuffered, textureData);
    }

    public void RequestSendTexture() {
        if (networkObject != null) {
            // networkObject.SendRpc(RPC_SEND_FULL_TEXTURE, Receivers.AllBuffered, paintCanvas.GetAllTextureData());
            // Testing
            byte[] textureData = paintCanvas.GetAllTextureData();
            // paintCanvas.SetAllTextureData(textureData.Compress());
            print("Sending texture data...");
            Projectile newProj = NetworkManager.Instance.InstantiateProjectile() as Projectile;
            newProj.tempTextureData = textureData.Compress();
        }
    }
}