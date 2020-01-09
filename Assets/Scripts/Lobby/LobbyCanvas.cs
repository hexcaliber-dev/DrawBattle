using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shared canvas in lobby. 
public class LobbyCanvas : MonoBehaviour {
    public Texture2D texture { get; private set; }

    // No customization in the lobby- everyone gets normal size
    public const int BRUSH_SIZE = 5;

    /// Fineness of angle sweeping when drawing circles. Smaller = more precise circles; larger = faster
    public float angleStep = 0.1f;

    // How many seconds in between canvas updates
    public float updateInterval = 1 / 60f;

    /// Flattened array of pixels and their colors. See https://docs.unity3d.com/ScriptReference/Texture2D.SetPixels32.html for more info
    /// When dealing with the canvas, DO NOT directly edit the texture! It will be overwritten. Instead, edit cur_colors.
    private Color32[] cur_colors;

    // True if using 3:2 or taller
    private bool tallAspectRatio = false;

    /// Keep track of window resize events to reposition canvas
    private Vector2 resolution;

    public byte[] GetAllTextureData() {
        return texture.GetRawTextureData();
    }

    private void Start() {
        PrepareTemporaryTexture();
        StartCoroutine(UpdateCanvas(updateInterval));
    }

    private void Awake() {
        resolution = new Vector2(Screen.width, Screen.height);
        Recenter();
    }

    private void Update() {
        // Fill screen
        if (resolution.x != Screen.width || resolution.y != Screen.height || transform.position != Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 5f))) {
            // Reposition at center of screen
            Recenter();

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
    }

    // Center the canvas
    private void Recenter() {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 5f));
    }

    // Run on Start to create an empty texture. Cannot be used to reset the canvas.
    private void PrepareTemporaryTexture() {
        texture = (Texture2D) GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    /// Creates a single dot at a location given a color and brush size
    public void BrushAreaWithColor(Vector2 pixelUV, Color color) {
        // Polar coords are cool and good (plus they are ok at drawing circles i guess)
        for (int r = 0; r < BRUSH_SIZE; r++) {
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