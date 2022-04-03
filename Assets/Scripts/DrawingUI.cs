using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour {
    public DrawingSystem drawingSystem;
    public RectTransform mainContainer;
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
        drawingSystem.Cancel();
    }

    private void ChangeBrushSize(int change) {
        drawingSystem.BrushSize += change;
        brushSizeText.text = drawingSystem.BrushSize.ToString();
    }

    private void SetBrushColor(Color color) {
        drawingSystem.BrushColor = color;
    }

    private Coroutine moveRoutine;
    public void Move(bool moveIn) {
        if (moveIn) {
            SetBrushColor(brushColors[0]);
            gameObject.SetActive(true);
        }
        this.EnsureCoroutineStopped(ref moveRoutine);
        Vector2 startPos = mainContainer.anchoredPosition;
        Vector2 endPos = moveIn ? Vector2.zero : new Vector2(0, -1050);
        moveRoutine = this.CreateAnimationRoutine(DrawingSystem.ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, easedProgress);
        }, () => {
            if (!moveIn) {
                gameObject.SetActive(false);
            }
        });
    }
}
