using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerDraw : PlayerDrawBehavior {

    public PaintCanvas paintCanvas;

    /// Fineness of angle sweeping when drawing circles. Smaller = more precise circles; larger = faster
    public float ANGLE_STEP = 0.1f;

    /// How much distance should be skipped in between lerping. (For filling in gaps when cursor is moving quickly)
    public float LERP_STEP = 0.05f;

    /// Minumum distance between successive cursor positions in order to fill in the middle
    public float LERP_THRESHOLD = 100f;

    Vector2 prevPos;
    bool isDragging = false;

    private void Start() {
        var data = paintCanvas.GetAllTextureData();
        var zippeddata = data.Compress();

        // RpcSendFullTexture(zippeddata);
    }

    private void Update() {

        if(Input.GetMouseButtonUp(0)) isDragging = false;

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                var pallet = hit.collider.GetComponent<PaintCanvas>();
                if (pallet != null) {
                    // Debug.Log(hit.textureCoord);
                    // Debug.Log(hit.point);

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

                    // if (firstMarkMade && Vector2.Distance(prevPos, pixelUV) > LERP_THRESHOLD) {
                    //     for (
                    //         float lerpVal = LERP_STEP; lerpVal < 1f; lerpVal += LERP_STEP
                    //     ) {
                    //         BrushAreaWithColor(prevPos * (1 - lerpVal) + pixelUV * lerpVal, Color.black, 10);
                    //     }
                    // }

                    if(!isDragging)
                        prevPos = pixelUV;
                    else
                        ColorBetween(prevPos, pixelUV, Color.black, 10);

                    BrushAreaWithColor(pixelUV, Color.black, 10);
                    prevPos = pixelUV;
                    isDragging = true;
                }
            }
        }
    }

    public void ColorBetween(Vector2 start_point, Vector2 end_point, Color color, int width) {
        // Get the distance from start to finish
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
        float lerp_steps = 1 / distance * 10;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps) {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            BrushAreaWithColor(cur_position, color, width);
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
        // for (int x = -size; x < size; x++) {
        //     for (int y = -size; y < size; y++) {
        //         paintCanvas.Texture.SetPixel((int) pixelUV.x + x, (int) pixelUV.y + y, color);
        //     }
        // }
        for (int r = 0; r < size; r++) {
            for (float angle = 0; angle < Mathf.PI * 2; angle += ANGLE_STEP) {
                float x1 = r * Mathf.Cos(angle);
                float y1 = r * Mathf.Sin(angle);
                paintCanvas.Texture.SetPixel((int) (pixelUV.x + x1), (int) (pixelUV.y + y1), color);
            }
        }

        paintCanvas.Texture.Apply();
    }
}