using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
    public Transform t;
    public PlayerUI playerUI;
    public DrawingSystem drawingSystem;
    public Camera mainCamera;
    private Transform mainCameraTransform;

    public CharacterController characterController;
    public FirstPersonController firstPersonController;

    private void Awake() {
        mainCameraTransform = mainCamera.transform;
    }

    private PaintingCanvas currentCanvas;
    private InteractiveObject currentObject;
    private InteractiveObject CurrentObject {
        get {
            return currentObject;
        }
        set {
            if(currentObject != value) {
                currentObject = value;
                playerUI.RefreshForObject(currentObject);
            }
        }
    }

    private void Update() {
        if(Time.frameCount % 6 == 0) {
            InteractiveObject nearestObject = InteractiveObjectManager.Instance.GetNearestObject(t.position, mainCameraTransform.forward);
            CurrentObject = nearestObject;
            currentCanvas = nearestObject as PaintingCanvas;
        }
        if(currentCanvas != null && Input.GetKeyDown(KeyCode.E)) {
            enabled = false;
            characterController.enabled = false;
            firstPersonController.enabled = false;
            playerUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            drawingSystem.DrawToCanvas(currentCanvas, mainCamera, () => {
                enabled = true;
                characterController.enabled = true;
                firstPersonController.enabled = true;
                playerUI.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });
        }
    }
}
