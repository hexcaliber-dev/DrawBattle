using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour {
    public Image colorDisplay, lightSliderBackground;
    public GameObject sliderPanel;
    public Slider hueSlider, lightSlider;

    public Color currColor;

    // Start is called before the first frame update
    void Awake() {
        lightSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });
        hueSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });
    }

    // Update is called once per frame
    void Update() {

    }

    void OnSliderChange() {
        currColor = Color.HSVToRGB(hueSlider.value, lightSlider.value, lightSlider.value);
        colorDisplay.color = currColor;
        lightSliderBackground.color = currColor;
    }
}