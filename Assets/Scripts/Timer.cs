using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : TimerBehavior {

    // Amount of time given to each phase (MAINMENU, LOBBY, DRAW, BATTLE, VOTE). -1 means this phase is untimed.
    static readonly int[] STARTING_TIMES = {-1, -1, 3, 3, 3 };

    [SerializeField]
    Text timerText;
    [SerializeField]
    Image fillImg;

    IEnumerator currCountdown;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(transform.parent.gameObject);
        SceneManager.sceneLoaded += ManageTimerOnSceneLoad;
    }

    // Update is called once per frame
    void Update() {
        if (networkObject != null) {
            timerText.text = networkObject.timeRemaining.ToString();
        }
    }

    void ManageTimerOnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (STARTING_TIMES[scene.buildIndex] > 0) {
            GetComponent<CanvasGroup>().alpha = 1f;
            if (ServerInfo.isServer) {
                if (currCountdown != null)
                    StopCoroutine(currCountdown);
                currCountdown = CountDown(STARTING_TIMES[scene.buildIndex], scene.buildIndex);
                StartCoroutine(currCountdown);
            }
        } else {
            GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    // Should be run on server only
    IEnumerator CountDown(int startingTime, int buildIndex) {
        if (startingTime < 0) {
            Debug.LogError("CountDown called with starting time of -1!");
        }

        networkObject.timeRemaining = startingTime;

        // Do the fill animation
        fillImg.fillAmount = 0;
        DOTween.To(() => fillImg.fillAmount, x => fillImg.fillAmount = x, 1, startingTime).SetEase(Ease.Linear);

        if (ServerInfo.isServer) {
            while (networkObject.timeRemaining > 0) {
                networkObject.timeRemaining--;
                yield return new WaitForSeconds(1f);
            }
        }
        currCountdown = null;

        if (networkObject != null) {

            switch (buildIndex) {
                case (int) ServerInfo.GamePhase.Drawing:
                    // Send drawing to server
                    GameObject.FindObjectOfType<PlayerDraw>().networkObject.SendRpc(PlayerDrawBehavior.RPC_SEND_DRAWING_COMPLETE, Receivers.Server, ServerInfo.playerNum);
                    // Save drawing locally
                    byte[] textureData = GameObject.FindObjectOfType<PaintCanvas>().GetAllTextureData().Compress();
                    PlayerShoot.textureData = textureData;
                    break;

                case (int) ServerInfo.GamePhase.Battling:
                    SceneManager.LoadScene(4);
                    break;

                case (int) ServerInfo.GamePhase.Voting:
                    PlayerDraw.currDrawing++;
                    SceneManager.LoadScene(2);
                    break;
            }
        }
    }
}