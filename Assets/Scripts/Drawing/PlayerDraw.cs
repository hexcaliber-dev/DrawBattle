using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.UI;

/// Player brush behavior on the drawing phase. IS a networkObject!
public class PlayerDraw : PlayerDrawBehavior {

    ServerInfo serverInfo;
    public PaintCanvas paintCanvas;

    /// Last frame's mouse position. Used for lerping
    Vector2 prevPos;
    /// Used to keep track of mouseup/down events so we don't fill in the space between 2 points if there was a pen lift
    bool isDragging = false;
    public bool canDraw = true;
    int completedPlayers = 0;
    public bool eraserEnabled = false;

    public Sprite penEnabledImg, penDisabledImg, eraserEnabledImg, eraserDisabledImg, submittedImg;
    public Button penButton, eraserButton;
    public Button submitButton;
    public Text infoText;

    /// What the player is currently drawing. 0-homebase, 1-projectile, 2-tankbase, 3-tanktop, 4-barriers
    public static int currDrawing = -1;
    public static readonly string[] drawingNames = { "home base", "projectile", "tank base", "tank head", "barrier blocks" };
    public enum Drawings { HomeBase, Projectile, TankBase, TankHead, BarrierBlock }

    private void Start() {
        serverInfo = GameObject.FindObjectOfType<ServerInfo>();

    }

    protected override void NetworkStart() {
        base.NetworkStart();
        if (ServerInfo.isServer)
            GameObject.FindObjectOfType<PlayerDraw>().networkObject.SendRpc(PlayerDrawBehavior.RPC_SEND_SWITCH_TO_NEXT_DRAWING, Receivers.All);
    }

    private void Update() {

        // Pencil/eraser switching
        if (Input.GetKeyDown(KeyCode.P)) {
            SetEraserEnabled(false);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            SetEraserEnabled(true);
        }

        if (canDraw) {

            // Need to keep track of dragging to make sure lerping isn't done between penup/pendown
            if (Input.GetMouseButtonUp(0)) isDragging = false;

            if (Input.GetMouseButton(0)) {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    var pallet = hit.collider.GetComponent<PaintCanvas>();
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

                        Color currColor = new Color(0, 0, 0, 0);

                        if (!eraserEnabled)
                            currColor = GameObject.FindObjectOfType<ColorPicker>().currColor;

                        int currSize = GameObject.FindObjectOfType<BrushPicker>().brushSize;
                        if (!isDragging)
                            prevPos = pixelUV;
                        else
                            paintCanvas.ColorBetween(prevPos, pixelUV, currColor, currSize);

                        paintCanvas.BrushAreaWithColor(pixelUV, currColor, currSize);
                        prevPos = pixelUV;
                        isDragging = true;
                    }
                }
            }
        }

    }

    // Deprecated, use SendDrawingComplete() instead
    public override void SendFullTexture(RpcArgs args) {
        byte[] textureData = args.GetNext<byte[]>();
        paintCanvas.SetAllTextureData(textureData.Compress());
    }

    /// All-instance RPC. Server has additional tracking capabilities
    public override void SendDrawingComplete(RpcArgs args) {
        int playerNum = args.GetNext<int>();
        byte[] textureData = args.GetNext<byte[]>();

        if (ServerInfo.isServer) {
            completedPlayers++;
            print("Completed players: " + completedPlayers);

            if (completedPlayers == serverInfo.networkObject.numPlayers) {
                ServerInfo.ChangePhase(ServerInfo.GamePhase.Battling);
            }
        }

        // Save drawing locally
        DrawableTexture.textures[playerNum][(int) PlayerDraw.currDrawing] = textureData.Decompress();

    }

    /// All-instance RPC
    public override void SendSwitchToNextDrawing(RpcArgs args) {
        currDrawing++;
        infoText.text = "Draw your " + drawingNames[currDrawing];
    }

    // Run when submit button is clicked
    public void RequestCompleteDrawing() {
        if (networkObject != null) {
            byte[] textureData = paintCanvas.GetAllTextureData();

            // Send drawing to server (and save locally)
            networkObject.SendRpc(RPC_SEND_DRAWING_COMPLETE, Receivers.AllBuffered, ServerInfo.playerNum, textureData.Compress());

            // Update button
            submitButton.image.sprite = submittedImg;

            // Update dot
            GameObject.FindObjectOfType<LobbyDots>().networkObject.SendRpc(LobbyDotsBehavior.RPC_UPDATE_DOT, Receivers.AllBuffered, ServerInfo.playerNum - 1, 2);

            // Disable drawing
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            canDraw = false;
        }
    }

    // Toggles eraser.
    public void SetEraserEnabled(bool newVal) {
        eraserEnabled = newVal;

        if (eraserEnabled) {
            eraserButton.image.sprite = eraserEnabledImg;
            penButton.image.sprite = penDisabledImg;
        } else {
            eraserButton.image.sprite = eraserDisabledImg;
            penButton.image.sprite = penEnabledImg;
        }
    }
}