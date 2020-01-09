using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.UI;

/// Player behavior in the lobby. Is a NetworkObject.
public class LobbyPlayer : LobbyPlayerBehavior {

    public LobbyCanvas lobbyCanvas;

    /// Last frame's mouse position. Used for lerping
    Vector2 prevPos;

    /// Used to keep track of mouseup/down events so we don't fill in the space between 2 points if there was a pen lift
    bool isDragging = false;

    int readyPlayers = 0;

    ServerInfo serverInfo;

    public Button readyButton;
    public Sprite readyButtonSubmitted;

    // Indices correspond with player numbers
    readonly Color[] PLAYER_COLOR_PRESETS = { Color.red, Color.yellow, Color.green, Color.blue };

    private void Start() {
        serverInfo = GameObject.FindObjectOfType<ServerInfo>();
    }

    private void Update() {

        // Need to keep track of dragging to make sure lerping isn't done between penup/pendown
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                var pallet = hit.collider.GetComponent<LobbyCanvas>();
                if (pallet != null) {

                    // Check to make sure the canvas was clicked
                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    MeshCollider meshCollider = hit.collider as MeshCollider;

                    if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                        return;

                    Texture2D tex = rend.material.mainTexture as Texture2D;
                    Vector2 pixelUV = hit.textureCoord;

                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    if (!isDragging)
                        prevPos = pixelUV;
                    else {
                        // ColorBetween behavior
                        // Get the distance from start to finish
                        float distance = Vector2.Distance(prevPos, pixelUV);
                        Vector2 direction = (prevPos - pixelUV).normalized;

                        Vector2 cur_position = prevPos;

                        // Calculate how many times we should interpolate between prevPos and pixelUV based on the amount of time that has passed since the last update
                        float lerp_steps = 1 / distance * LobbyCanvas.BRUSH_SIZE;

                        for (float lerp = 0; lerp <= 1; lerp += lerp_steps) {
                            cur_position = Vector2.Lerp(prevPos, pixelUV, lerp);
                            lobbyCanvas.BrushAreaWithColor(cur_position, PLAYER_COLOR_PRESETS[ServerInfo.playerNum - 1]);
                            networkObject.SendRpc(RPC_DRAW, Receivers.All, ServerInfo.playerNum, cur_position);
                        }
                    }

                    lobbyCanvas.BrushAreaWithColor(pixelUV, PLAYER_COLOR_PRESETS[ServerInfo.playerNum - 1]);
                    // networkObject.SendRpc(RPC_DRAW, Receivers.All, ServerInfo.playerNum, pixelUV);
                    prevPos = pixelUV;
                    isDragging = true;
                }
            }
        }
    }

    // (All-instance RPC) A player requested to draw something at this spot
    public override void Draw(RpcArgs args) {
        Color color = PLAYER_COLOR_PRESETS[args.GetNext<int>() - 1];
        Vector2 pos = args.GetNext<Vector2>();
        lobbyCanvas.BrushAreaWithColor(pos, color);
    }

    /// Should be run ONLY by the server.
    public override void PlayerReady(RpcArgs args) {
        int playerNum = args.GetNext<int>();

        if (ServerInfo.isServer) {
            readyPlayers++;

            if (readyPlayers == serverInfo.networkObject.numPlayers) {
                serverInfo.networkObject.SendRpc(ServerInfo.RPC_CHANGE_PHASE, Receivers.All, (int) ServerInfo.GamePhase.Drawing);
            }
        } else {
            Debug.LogError("Server-only RPC PlayerReady was called on a client!");
        }
    }

    // Run when ready button is clicked
    public void RequestReady() {
        if (networkObject != null) {
            // Do an RPC call to let the server know
            networkObject.SendRpc(RPC_PLAYER_READY, Receivers.Server, ServerInfo.playerNum);

            // Update button
            readyButton.image.sprite = readyButtonSubmitted;
        }
    }
}