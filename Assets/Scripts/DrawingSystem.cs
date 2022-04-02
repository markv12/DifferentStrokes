using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class DrawingSystem : MonoBehaviour {
    public GameObject Brush;
    public float BrushSize = 0.1f;
    public RenderTexture RTexture;
    public Camera mainCamera;

    public Camera renderCamera;
    public Transform renderCameraTransform;

    public LayerMask layerMask;

    private bool inDrawingMode = false;

    private PaintingCanvas currentCanvas;
    private Action onExit;

    private Transform mainCameraTransform;
    private Vector3 mainCameraOriginalPos;
    private Quaternion mainCameraOriginalRotation;


    public void DrawToCanvas(PaintingCanvas paintingCanvas, Action _onExit) {
        currentCanvas = paintingCanvas;
        mainCameraTransform = mainCamera.transform;
        onExit = _onExit;

        mainCameraOriginalPos = mainCameraTransform.localPosition;
        mainCameraOriginalRotation = mainCameraTransform.localRotation;

        MoveCameraInFrontOfCanvas();
    }

    void Update() {
        if (inDrawingMode) {
            if (Input.GetMouseButton(0)) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction*4, Color.red, 1000);
                if (Physics.Raycast(ray, out RaycastHit hit, 10, layerMask)) {
                    GameObject go = Instantiate(Brush, hit.point, Quaternion.identity, transform);
                    go.transform.localScale = Vector3.one * BrushSize;
                    //renderCamera.Render();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ReturnCameraToPlayer();
            }
        }
    }

    private void MoveCameraInFrontOfCanvas() {
        Vector3 startPos = mainCameraTransform.position;
        Quaternion startRotation = mainCameraTransform.rotation;
        Vector3 endPos = currentCanvas.transform.position + (currentCanvas.transform.forward * -2.05f);
        Quaternion endRotation = Quaternion.Euler(-currentCanvas.transform.forward);
        this.CreateAnimationRoutine(1f, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, easedProgress), Quaternion.Lerp(startRotation, endRotation, easedProgress));
        }, () => {
            inDrawingMode = true;
        });
    }

    private void ReturnCameraToPlayer() {
        inDrawingMode = false;
        Vector3 startPos = mainCameraTransform.localPosition;
        Quaternion startRotation = mainCameraTransform.localRotation;
        Vector3 endPos = mainCameraOriginalPos;
        this.CreateAnimationRoutine(1f, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.localPosition = Vector3.Lerp(startPos, mainCameraOriginalPos, easedProgress);
            mainCameraTransform.localRotation = Quaternion.Lerp(startRotation, mainCameraOriginalRotation, easedProgress);
        }, () => {
            onExit?.Invoke();
        });
    }

    public void Save() {
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave() {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        Debug.Log(Application.dataPath + "/savedImage.png");

        //set active texture
        RenderTexture.active = RTexture;

        //convert rendering texture to texture2D
        var texture2D = new Texture2D(RTexture.width, RTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        texture2D.Apply();

        //write data to file
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);
    }
}
