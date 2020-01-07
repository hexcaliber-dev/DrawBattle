using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNavigation : MonoBehaviour
{
    public CanvasGroup mainPanel, localPanel, onlinePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePanel(CanvasGroup panelToToggle) {
        panelToToggle.alpha = (panelToToggle.alpha + 1) % 2;
        panelToToggle.blocksRaycasts = !panelToToggle.blocksRaycasts;
    }
}
