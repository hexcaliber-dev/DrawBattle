using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushPicker : MonoBehaviour {

    public Button brushSliderToggle;
    public GameObject sliderPanel;
    public Slider brushSizeSlider;
    public Image brushPreview;

    public int brushSize = 6;

    // Start is called before the first frame update
    void Start() {
        brushSizeSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });
        brushSizeSlider.value = brushSize;
    }

    // Update is called once per frame
    void Update() {

    }

    void OnSliderChange() {
        brushSize = (int)brushSizeSlider.value;
        brushPreview.rectTransform.sizeDelta = new Vector2(brushSize * 4, brushSize * 4);
    }

    public void ToggleSlider() {
        sliderPanel.SetActive(!sliderPanel.activeSelf);
    }
}