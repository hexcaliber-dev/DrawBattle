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
    static readonly int[] STARTING_TIMES = {-1, -1, 30, 60, 30 };

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
            // Do the fill animation
            fillImg.fillAmount = 0;
            DOTween.To(() => fillImg.fillAmount, x => fillImg.fillAmount = x, 1, STARTING_TIMES[scene.buildIndex]).SetEase(Ease.Linear);
        } else {
            GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    // Should be run on server only
    IEnumerator CountDown(int startingTime, int buildIndex) {
        if (ServerInfo.isServer) {
            if (startingTime < 0) {
                Debug.LogError("CountDown called with starting time of -1!");
            }

            networkObject.timeRemaining = startingTime;

            while (networkObject.timeRemaining > 0) {
                networkObject.timeRemaining--;
                yield return new WaitForSeconds(1f);
            }

            currCountdown = null;

            if (networkObject != null) {

                switch (buildIndex) {
                    case (int) ServerInfo.GamePhase.Drawing:
                        // Send drawing to server
                        GameObject.FindObjectOfType<PlayerDraw>().networkObject.SendRpc(PlayerDrawBehavior.RPC_SEND_DRAWING_COMPLETE, Receivers.Server, ServerInfo.playerNum);
                        // Save drawing locally
                        byte[] textureData = GameObject.FindObjectOfType<PaintCanvas>().GetAllTextureData().Compress();
                        DrawableTexture.textures[(int) PlayerDraw.currDrawing] = textureData;
                        break;

                    case (int) ServerInfo.GamePhase.Battling:
                        ServerInfo.ChangePhase(ServerInfo.GamePhase.Voting);
                        break;

                    case (int) ServerInfo.GamePhase.Voting:
                        // PlayerDraw.currDrawing++; done in NetworkStart in PlayerDraw
                        ServerInfo.ChangePhase(ServerInfo.GamePhase.Drawing);
                        break;
                }
            }
        } else {
            Debug.LogError("CountDown was called on a client!!");
        }
    }
}