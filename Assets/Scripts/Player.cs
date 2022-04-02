using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
    public Transform t;
    public PlayerUI playerUI;
    public DrawingSystem drawingSystem;

    public CharacterController characterController;
    public FirstPersonController firstPersonController;

    private PaintingCanvas currentCanvas;
    private PaintingCanvas CurrentCanvas {
        get {
            return currentCanvas;
        }
        set {
            if(currentCanvas != value) {
                currentCanvas = value;
                playerUI.RefreshForCanvas(currentCanvas);
            }
        }
    }

    private void Update() {
        if(Time.frameCount % 6 == 0) {
            CurrentCanvas = PaintingCanvasManager.Instance.GetNearestCanvas(t.position);
        }
        if(CurrentCanvas != null && Input.GetMouseButtonDown(0)) {
            enabled = false;
            characterController.enabled = false;
            firstPersonController.enabled = false;
            playerUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            drawingSystem.DrawToCanvas(CurrentCanvas, () => {
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
