using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    void Update() {

    }

    void ChangeHue() {
        currColor = Color.HSVToRGB(hueSlider.value, lightSlider.value, lightSlider.value);
        UpdateColorDisplays();
    }

    void ChangeLightness() {
        float tempH, tempS, _;
        Color.RGBToHSV(currColor, out tempH, out tempS, out _);
        currColor = Color.HSVToRGB(tempH, tempS, lightSlider.value);
        UpdateColorDisplays();
    }

    void UpdateColorDisplays() {
        colorDisplay.color = currColor;

        float tempH, tempS, _;
        Color.RGBToHSV(currColor, out tempH, out tempS, out _);
        lightSliderBackground.color = Color.HSVToRGB(tempH, tempS, 1);
    }

    public void ChangeColor(Color newColor) {
        currColor = newColor;
        UpdateColorDisplays();
    }

    public void ToggleSliders() {
        sliderPanel.SetActive(!sliderPanel.activeSelf);
    }
}