using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Displays the status of players in the lobby on the palette
public class LobbyDots : LobbyDotsBehavior {

    /// BOUNCING DOTS ANIMATION
    /// Credits: https://gist.github.com/reidscarboro/588911e7bc0e0ad82bfa8a1ad2397bd5

    //the total time of the animation
    public float repeatTime = 1;

    //the time for a dot to bounce up and come back down
    public float bounceTime = 0.25f;

    //how far does each dot move
    public float bounceHeight = 10;

    public List<Image> playerIcons;
    public List<Image> playerReadyIcons;

    int[] dotStates = new int[8];

    void Start() {
        for (int i = 0; i < playerIcons.Count; i++) {
            StartCoroutine(AnimateDots(playerIcons[i]));
            if (i == 0)
                ApplyDotState(i, 1);
            else
                ApplyDotState(i, 0);
        }
    }

    protected override void NetworkStart() {
        base.NetworkStart();
        if (ServerInfo.isServer && SceneManager.GetActiveScene().buildIndex == (int) ServerInfo.GamePhase.Drawing) {
            for (int i = 0; i < GameObject.FindObjectOfType<ServerInfo>().networkObject.numPlayers; i++) {
                networkObject.SendRpc(RPC_UPDATE_DOT, Receivers.AllBuffered, i, 1);
            }
        }
    }

    IEnumerator AnimateDots(Image playerIcon) {
        while (true) {
            int dotIndex = 0;
            if (playerIcon.gameObject.activeSelf) {
                foreach (Transform dot in playerIcon.GetComponentsInChildren<Transform>()) {
                    if (dot != playerIcon.transform) {
                        dot
                            .DOMoveY(dot.transform.position.y + bounceHeight, bounceTime / 2)
                            .SetDelay(dotIndex * bounceTime / 2)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() => {
                                dot.transform
                                    .DOMoveY(dot.transform.position.y - bounceHeight, bounceTime / 2)
                                    .SetEase(Ease.InQuad);
                            });
                        dotIndex++;
                    }
                }
            }
            yield return new WaitForSeconds(repeatTime);
        }
    }

    public override void UpdateDot(RpcArgs args) {
        int dotNum = args.GetNext<int>();
        print("Update dot for player" + (dotNum + 1));

        // 0 is hidden, 1 is waiting, 2 is ready
        int dotState = args.GetNext<int>();

        ApplyDotState(dotNum, dotState);
    }

    void ApplyDotState(int dotNum, int newState) {
        if (playerIcons.Count > dotNum) {
            dotStates[dotNum] = newState;
            if(playerIcons[dotNum] != null) {
                switch (newState) {
                    case 0: // Hidden
                        playerIcons[dotNum].gameObject.SetActive(false);
                        playerReadyIcons[dotNum].gameObject.SetActive(false);
                        break;
                    case 1: // Waiting
                        playerIcons[dotNum].gameObject.SetActive(true);
                        playerReadyIcons[dotNum].gameObject.SetActive(false);
                        break;
                    case 2: // Ready
                        playerIcons[dotNum].gameObject.SetActive(false);
                        playerReadyIcons[dotNum].gameObject.SetActive(true);
                        break;
                    default:
                        Debug.LogError("Invalid dot state in UpdateDot(): " + newState + " passed for dot " + dotNum + ". dotState must be 0, 1, or 2");
                        break;
                }
            }
        }
    }

    public override void RemoveDot(RpcArgs args) {
        int numPlayers = GameObject.FindObjectOfType<ServerInfo>().networkObject.numPlayers - 1;
        int dotToRemove = args.GetNext<int>();
        for (int i = dotToRemove; i < numPlayers; i++) {
            ApplyDotState(i, dotStates[i + 1]);
        }

        ApplyDotState(numPlayers + 1, 0);
    }
}