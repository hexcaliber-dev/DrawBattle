using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanvas : MonoBehaviour {
    public Texture2D texture { get; private set; }

    /// Fineness of angle sweeping when drawing circles. Smaller = more precise circles; larger = faster
    public float angleStep = 0.1f;

    // How many seconds in between canvas updates
    public float updateInterval = 1 / 60f;

    /// Flattened array of pixels and their colors. See https://docs.unity3d.com/ScriptReference/Texture2D.SetPixels32.html for more info
    private Color32[] cur_colors;

    public byte[] GetAllTextureData() {
        return texture.GetRawTextureData();
    }

    private void Start() {
        PrepareTemporaryTexture();
        StartCoroutine(UpdateCanvas(updateInterval));
    }

    private void PrepareTemporaryTexture() {
        texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    internal void SetAllTextureData(byte[] textureData) {
        texture.LoadRawTextureData(textureData);
        texture.Apply();
    }

    /// Creates a single dot at a location given a color and brush size
    public void BrushAreaWithColor(Vector2 pixelUV, Color color, int size) {
        // Polar coords are cool and good (plus they are ok at drawing circles i guess)
        for (int r = 0; r < size; r++) {
            for (float angle = 0; angle < Mathf.PI * 2; angle += angleStep) {
                float x1 = r * Mathf.Cos(angle);
                float y1 = r * Mathf.Sin(angle);

                // Need to transform x and y coordinates to flat coordinates of array
                int array_pos = (int) (texture.width * (int) (pixelUV.y + y1) + (pixelUV.x + x1));

                // Check if this is a valid position
                if (array_pos > cur_colors.Length || array_pos < 0)
                    return;

                // Update pixel array
                cur_colors[array_pos] = color;
            }
        }
    }

    /// Lerps between two points, filling in the gaps with color. Adapted from https://assetstore.unity.com/packages/tools/painting/free-draw-simple-drawing-on-sprites-2d-textures-113131#content
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

    /// Applies changes to canvas. Smaller updateInterval = faster updates, but makes the game laggier
    private IEnumerator UpdateCanvas(float updateInterval) {
        while (true) {
            if (texture != null) {
                if (cur_colors != null) {
                    texture.SetPixels32(cur_colors);
                    texture.Apply();
                }
                cur_colors = texture.GetPixels32();
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }
}