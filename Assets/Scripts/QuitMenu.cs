using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePanel();
        }
    }

    public void TogglePanel() {
        CanvasGroup c = GetComponent<CanvasGroup>();
        c.blocksRaycasts = !c.blocksRaycasts;
        c.alpha = (c.alpha + 1) % 2;
    }

    public void QuitGame() {
        Application.Quit(0);
    }

    public void QuitToMainMenu() {
        GameObject.FindObjectOfType<NetworkManager>().Disconnect();
        SceneManager.LoadScene(0);
        print("Disconnected from Server: Player Quit");
    }
}