using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawingSystem : MonoBehaviour {
    public DrawingBrush brush;
    public RenderTexture RTexture;

    public Camera renderCamera;
    public Transform renderCameraTransform;

    public DrawingUI drawingUI;

    public LayerMask layerMask;

    private PaintingCanvas currentCanvas;
    private Action onExit;

    private Camera mainCamera;
    private Transform mainCameraTransform;
    private Vector3 mainCameraOriginalPos;
    private Quaternion mainCameraOriginalRotation;

    private bool inDrawingMode = false;
    private bool InDrawingMode {
        get {
            return inDrawingMode;
        }
        set {
            inDrawingMode = value;
        }
    }

    private int brushSize = 1;
    public int BrushSize {
        get {
            return brushSize;
        }
        set {
            brushSize = Mathf.Max(0, Mathf.Min(5, value));
        }
    }

    private Color brushColor = Color.white;
    public Color BrushColor {
        get {
            return brushColor;
        }
        set {
            brushColor = value;
        }
    }

    private static readonly float[] BRUSH_SIZES = new float[] { 0.025f, 0.05f, 0.1f, 0.3f, 0.5f, 1f };
    private Texture2D prevTex;
    public void DrawToCanvas(PaintingCanvas paintingCanvas, Camera _mainCamera, Action _onExit) {
        currentCanvas = paintingCanvas;
        prevTex = currentCanvas.GetCanvasTexture();
        mainCamera = _mainCamera;
        mainCameraTransform = _mainCamera.transform;
        onExit = _onExit;

        mainCameraOriginalPos = mainCameraTransform.localPosition;
        mainCameraOriginalRotation = mainCameraTransform.localRotation;

        MoveCameraInFrontOfCanvas();
    }

    private readonly List<DrawingBrush> brushInstances = new List<DrawingBrush>();
    private bool isDrawing = false;
    void Update() {
        if (InDrawingMode) {
            if (Input.GetMouseButtonDown(0)) {
                if (isDrawing == false) {
                    AudioManager.Instance.PlayBrushSound(UnityEngine.Random.Range(0.1f, 0.4f));
                }
                isDrawing = true;
            } else if (Input.GetMouseButtonUp(0)) {
                isDrawing = false;
                lastDrawPosition = Vector3.zero;
                WriteBrushesToTexture();
            }

            if (isDrawing) {
                Draw();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Cancel();
            }
        }
    }

    private void WriteBrushesToTexture() {
        StartCoroutine(WriteRoutine());

        IEnumerator WriteRoutine() {
            renderCamera.Render();
            currentCanvas.SetCanvasTexture(RTexture);
            yield return null;
            Clear();
        }
    }

    private Vector3 lastDrawPosition;
    private const float MAX_DOT_DISTANCE = 0.025f;
    private void Draw() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, layerMask)) {
            Transform canvasT = hit.collider.transform;
            Vector3 spawnPoint = hit.point + (canvasT.forward * -0.005f);
            float diffMagnitude = (spawnPoint - lastDrawPosition).magnitude;
            if (diffMagnitude > MAX_DOT_DISTANCE && lastDrawPosition != Vector3.zero) {
                FillInBetween(lastDrawPosition, spawnPoint, canvasT.rotation, diffMagnitude);
            }
            DrawDot(spawnPoint, canvasT.rotation);
            lastDrawPosition = spawnPoint;
        }
    }

    private void FillInBetween(Vector3 startPos, Vector3 endPos, Quaternion spawnRotation, float diffMagnitude) {
        int dotsToDraw = Mathf.CeilToInt(diffMagnitude / MAX_DOT_DISTANCE);
        for (int i = 1; i < dotsToDraw; i++) {
            float progress = (float)i / (float)dotsToDraw;
            Vector3 drawPos = Vector3.Lerp(startPos, endPos, progress);
            DrawDot(drawPos, spawnRotation);
        }
    }

    private void DrawDot(Vector3 spawnPoint, Quaternion spawnRotation) {
        DrawingBrush brushInstance = Instantiate(brush, spawnPoint, spawnRotation, transform);
        brushInstance.transform.localScale = Vector3.one * BRUSH_SIZES[BrushSize];
        brushInstance.SetColor(BrushColor);
        brushInstances.Add(brushInstance);
    }

    public const float ANIM_DURATION = 0.8f;
    private void MoveCameraInFrontOfCanvas() {
        drawingUI.Move(true);
        Vector3 startPos = mainCameraTransform.position;
        Quaternion startRotation = mainCameraTransform.rotation;
        Vector3 endPos = currentCanvas.transform.position + (currentCanvas.transform.forward * -2.05f);
        Quaternion endRotation = Quaternion.LookRotation(currentCanvas.transform.forward, currentCanvas.transform.up);
        renderCamera.transform.SetPositionAndRotation(endPos, endRotation);
        this.CreateAnimationRoutine(ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, easedProgress), Quaternion.Lerp(startRotation, endRotation, easedProgress));
        }, () => {
            InDrawingMode = true;
        });
    }

    public void ReturnCameraToPlayer() {
        drawingUI.Move(false);
        InDrawingMode = false;
        isDrawing = false;
        Vector3 startPos = mainCameraTransform.localPosition;
        Quaternion startRotation = mainCameraTransform.localRotation;
        Vector3 endPos = mainCameraOriginalPos;
        this.CreateAnimationRoutine(ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.localPosition = Vector3.Lerp(startPos, mainCameraOriginalPos, easedProgress);
            mainCameraTransform.localRotation = Quaternion.Lerp(startRotation, mainCameraOriginalRotation, easedProgress);
        }, () => {
            onExit?.Invoke();
        });
    }

    public void SubmitCurrentDrawing() {
        AudioManager.Instance.PlaySaveSound(0.5f);

        if (InDrawingMode) {
            Texture2D tex = currentCanvas.canvasSpriteRenderer.sprite.texture;
            switch (currentCanvas.PaintingStatus) {
                case PaintingStatus.Blank:
                    ArtNetworking.Instance.SendUnfinishedImage(tex);
                    currentCanvas.PaintingStatus = PaintingStatus.NeedsFixing;
                    break;
                case PaintingStatus.NeedsFixing:
                    ArtNetworking.Instance.SendFinishedImage(tex, currentCanvas.ImageID);
                    currentCanvas.PaintingStatus = PaintingStatus.Complete;
                    break;
            }
            currentCanvas.Locked = true;
            ReturnCameraToPlayer();
        }
    }

    public void Cancel() {
        AudioManager.Instance.PlayPaperFlip(0.4f);

        switch (currentCanvas.PaintingStatus) {
            case PaintingStatus.Blank:
                currentCanvas.SetCanvasTexture(WhiteTex);
                break;
            case PaintingStatus.NeedsFixing:
                currentCanvas.SetCanvasTexture(prevTex);
                break;
        }
        Clear();
        ReturnCameraToPlayer();
    }

    private Texture2D whiteTex;
    private Texture2D WhiteTex {
        get {
            if(whiteTex == null) {
                whiteTex = new Texture2D(512, 512);
                Color32[] pixels = whiteTex.GetPixels32();
                for (int i = 0; i < pixels.Length; i++) {
                    pixels[i] = new Color32(255, 255, 255, 255);
                }
                whiteTex.SetPixels32(pixels);
                whiteTex.Apply();
            }
            return whiteTex;
        }
    }

    private void Clear() {
        for (int i = 0; i < brushInstances.Count; i++) {
            Destroy(brushInstances[i].gameObject);
        }
        brushInstances.Clear();
    }
}
