using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
    public Transform t;
    public PlayerUI playerUI;
    public DrawingSystem drawingSystem;
    public DialogueSystem dialogueSystem;
    public Camera mainCamera;
    private Transform mainCameraTransform;

    public CharacterController characterController;
    public FirstPersonController firstPersonController;

    private void Awake() {
        mainCameraTransform = mainCamera.transform;
    }

    private PaintingCanvas currentCanvas;
    private NPC currentNPC;
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
            currentNPC = nearestObject as NPC;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentCanvas != null) {
                SetFPSControllerActive(false);
                AudioManager.Instance.PlayStartDrawingSound(0.5f);
                drawingSystem.DrawToCanvas(currentCanvas, mainCamera, () => {
                    SetFPSControllerActive(true);
                });
            } else if (currentNPC != null) {
                currentNPC.speechBubble.Fade(false);
                SetFPSControllerActive(false);
                dialogueSystem.TalkToNPC(currentNPC, mainCamera, () => {
                    SetFPSControllerActive(true);
                });
            }
        }
    }

    private void SetFPSControllerActive(bool isActive) {
        enabled = isActive;
        characterController.enabled = isActive;
        firstPersonController.enabled = isActive;
        playerUI.gameObject.SetActive(isActive);
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isActive;
    }
}
