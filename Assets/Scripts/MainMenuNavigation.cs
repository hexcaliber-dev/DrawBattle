using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls buttons and panels in the main menu
public class MainMenuNavigation : MonoBehaviour
{
    public CanvasGroup mainPanel, localPanel, onlinePanel;

    void Start(){}
    void Update() {}

    public void TogglePanel(CanvasGroup panelToToggle) {
        panelToToggle.alpha = (panelToToggle.alpha + 1) % 2;
        panelToToggle.blocksRaycasts = !panelToToggle.blocksRaycasts;
    }
}
