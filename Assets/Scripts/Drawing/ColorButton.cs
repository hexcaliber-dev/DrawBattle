using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour {
    public Color color;
    // Start is called before the first frame update
    void Start() {
        GetComponent<Button>().onClick.AddListener(ChangeColor);
    }

    void ChangeColor() {
        GameObject.FindObjectOfType<ColorPicker>().ChangeColor(new Color(color.r, color.g, color.b, 1));
    }
}