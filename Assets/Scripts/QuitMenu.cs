using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages quit panel behavior for all phases. 
public class QuitMenu : MonoBehaviour {

    // Start is called before the first frame update
    void Start() { }

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
        ServerInfo.expectingQuit = true;
        if (GameObject.FindObjectOfType<ServerInfo>().networkObject != null) {
            GameObject.FindObjectOfType<ServerInfo>().networkObject.SendRpc(ServerInfoBehavior.RPC_LEAVE_GAME, Receivers.All, ServerInfo.playerNum);
            GameObject.FindObjectOfType<NetworkManager>().Disconnect();
        }
        SceneManager.LoadScene(0);
        print("Disconnected from Server: Player Quit");
    }
}