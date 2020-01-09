using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Manages brush color selection
public class ColorPicker : MonoBehaviour {
    public Image colorDisplay, lightSliderBackground;
    public GameObject sliderPanel;
    public Slider hueSlider, lightSlider;

    public Color currColor = new Color(0, 0, 0, 1);

    // Start is called before the first frame update
    void Awake() {
        lightSlider.onValueChanged.AddListener(delegate { ChangeLightness(); });
        hueSlider.onValueChanged.AddListener(delegate { ChangeHue(); });
        UpdateColorDisplays();
    }

    // Update is called once per frame
    void Update() {}

    // Run on hue slider change
    void ChangeHue() {
        currColor = Color.HSVToRGB(hueSlider.value, lightSlider.value, lightSlider.value);
        UpdateColorDisplays();
    }

    // Run on lightness slider change
    void ChangeLightness() {

        // Only set Value in HSV, leave the rest alone
        float tempH, tempS, _;
        Color.RGBToHSV(currColor, out tempH, out tempS, out _);
        currColor = Color.HSVToRGB(tempH, tempS, lightSlider.value);
        UpdateColorDisplays();
    }

    // Changes the color display and lightness slider to reflect new color
    void UpdateColorDisplays() {
        colorDisplay.color = currColor;

        // Make sure the color is max value so there aren't wonky color jumps
        float tempH, tempS, _;
        Color.RGBToHSV(currColor, out tempH, out tempS, out _);
        lightSliderBackground.color = Color.HSVToRGB(tempH, tempS, 1);
    }

    // Change color to a preset color (for buttons)
    public void ChangeColor(Color newColor) {
        currColor = newColor;
        UpdateColorDisplays();
    }

    // Show/hide color sliders
    public void ToggleSliders() {
        sliderPanel.SetActive(!sliderPanel.activeSelf);
    }
}