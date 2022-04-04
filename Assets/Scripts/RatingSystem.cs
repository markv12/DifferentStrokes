using System;
using UnityEngine;

public class RatingSystem : MonoBehaviour {
    public RatingUI ratingUI;

    public PaintingCanvas currentCanvas;
    private Action onExit;

    private Camera mainCamera;
    private Transform mainCameraTransform;
    private Vector3 mainCameraOriginalPos;
    private Quaternion mainCameraOriginalRotation;

    private bool inRatingMode = false;
    private bool InRatingMode {
        get {
            return inRatingMode;
        }
        set {
            inRatingMode = value;
        }
    }

    public void EnterRatingMode(PaintingCanvas paintingCanvas, Transform _mainCameraTransform, Action _onExit) {
        currentCanvas = paintingCanvas;
        mainCameraTransform = _mainCameraTransform;
        onExit = _onExit;

        mainCameraOriginalPos = mainCameraTransform.localPosition;
        mainCameraOriginalRotation = mainCameraTransform.localRotation;

        MoveCameraInFrontOfCanvas();
    }

    public void Like() {
        ArtNetworking.Instance.SendLike(currentCanvas.ImageID);
        currentCanvas.Locked = true;
        ReturnCameraToPlayer();
    }

    public void Dislike() {
        ArtNetworking.Instance.SendDislike(currentCanvas.ImageID);
        currentCanvas.Locked = true;
        ReturnCameraToPlayer();
    }

    public void ShowOriginal() {
        currentCanvas.ShowOriginalTexture();
    }

    public void ShowCompleted() {
        currentCanvas.ShowCompleteTexture();
    }

    void Update() {
        if (InRatingMode) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ShowCompleted();
                ReturnCameraToPlayer();
            }
        }
    }

    public const float ANIM_DURATION = 0.8f;
    private void MoveCameraInFrontOfCanvas() {
        ratingUI.Move(true);
        Vector3 startPos = mainCameraTransform.position;
        Quaternion startRotation = mainCameraTransform.rotation;
        Vector3 endPos = currentCanvas.transform.position + (currentCanvas.transform.forward * -2.05f);
        Quaternion endRotation = Quaternion.LookRotation(currentCanvas.transform.forward, currentCanvas.transform.up);
        this.CreateAnimationRoutine(ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, easedProgress), Quaternion.Lerp(startRotation, endRotation, easedProgress));
        }, () => {
            InRatingMode = true;
        });
    }

    public void ReturnCameraToPlayer() {
        ratingUI.Move(false);
        InRatingMode = false;
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

    public void Cancel() {
        ReturnCameraToPlayer();
    }
}
