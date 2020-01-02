using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerDraw : PlayerDrawBehavior {

    public PaintCanvas paintCanvas;

    /// Fineness of angle sweeping when drawing circles. Smaller = more precise circles; larger = faster
    public float angleStep = 0.1f;

    // How many seconds in between canvas updates
    public float updateInterval = 0.25f;

    Vector2 prevPos;
    bool isDragging = false;

    Color32[] cur_colors;

    private void Start() {

        StartCoroutine(UpdateCanvas(updateInterval));
        var data = paintCanvas.GetAllTextureData();
        var zippeddata = data.Compress();

        // RpcSendFullTexture(zippeddata);
    }

    private void Update() {

        if (Input.GetMouseButtonUp(0)) isDragging = false;

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

                    if (!isDragging)
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
            for (float angle = 0; angle < Mathf.PI * 2; angle += angleStep) {
                float x1 = r * Mathf.Cos(angle);
                float y1 = r * Mathf.Sin(angle);

                // Need to transform x and y coordinates to flat coordinates of array
                int array_pos = (int) (paintCanvas.Texture.width * (int) (pixelUV.y + y1) + (pixelUV.x + x1));

                // Check if this is a valid position
                if (array_pos > cur_colors.Length || array_pos < 0)
                    return;

                cur_colors[array_pos] = color;
                // ((int) (pixelUV.x + x1), (int) (pixelUV.y + y1), color);
            }
        }

    }

    private IEnumerator UpdateCanvas(float updateInterval) {
        while (true) {
            if (paintCanvas.Texture != null) {
                if(cur_colors != null) {
                    paintCanvas.Texture.SetPixels32(cur_colors);
                    paintCanvas.Texture.Apply();
                }
                cur_colors = paintCanvas.Texture.GetPixels32();
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }
}