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

    public Color[] brushColors;
    public Button[] brushColorSquares;
    public Image[] brushColorImages;

    private void Awake() {
        submitButton.onClick.AddListener(Submit);
        cancelButton.onClick.AddListener(Cancel);

        brushSizeUpButton.onClick.AddListener(() => ChangeBrushSize(1));
        brushSizeDownButton.onClick.AddListener(() => ChangeBrushSize(-1));

        for (int i = 0; i < brushColorSquares.Length; i++) {
            Color c = brushColors[i];
            brushColorSquares[i].onClick.AddListener(() => { SetBrushColor(c); });
            brushColorImages[i].color = c;
        }
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

    private void SetBrushColor(Color color) {
        drawingSystem.BrushColor = color;
    }
}
