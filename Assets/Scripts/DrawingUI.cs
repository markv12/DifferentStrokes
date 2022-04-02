using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour {
    public Button submitButton;
    public Button cancelButton;
    public DrawingSystem drawingSystem;

    private void Awake() {
        submitButton.onClick.AddListener(Submit);
        cancelButton.onClick.AddListener(Cancel);
    }

    private void Submit() {
        drawingSystem.ReturnCameraToPlayer();
    }

    private void Cancel() {
        drawingSystem.Clear();
        drawingSystem.ReturnCameraToPlayer();
    }
}
