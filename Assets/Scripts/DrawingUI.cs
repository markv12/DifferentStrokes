using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour {
    public DrawingSystem drawingSystem;
    public Button submitButton;
    public Button cancelButton;

    public Button brushSizeUpButton;
    public Button brushSizeDownButton;
    public TMP_Text brushSizeText;

    private void Awake() {
        submitButton.onClick.AddListener(Submit);
        cancelButton.onClick.AddListener(Cancel);

        brushSizeUpButton.onClick.AddListener(() => ChangeBrushSize(1));
        brushSizeDownButton.onClick.AddListener(() => ChangeBrushSize(-1));
    }

    private void Submit() {
        drawingSystem.SubmitCurrentDrawing();
    }

    private void Cancel() {
        drawingSystem.Clear();
        drawingSystem.ReturnCameraToPlayer();
    }

    private void ChangeBrushSize(int change) {
        drawingSystem.BrushSize += change;
        brushSizeText.text = drawingSystem.BrushSize.ToString();
    }
}
