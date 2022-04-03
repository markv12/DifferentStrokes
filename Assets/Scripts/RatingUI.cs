using UnityEngine;
using UnityEngine.UI;

public class RatingUI : MonoBehaviour {
    public RatingSystem ratingSystem;
    public RectTransform mainContainer;
    public Button likeButton;
    public Button dislikeButton;
    public Button showBackButton;
    public Button showFrontButton;

    private void Awake() {
        likeButton.onClick.AddListener(Like);
        dislikeButton.onClick.AddListener(Dislike);
        showBackButton.onClick.AddListener(ShowBack);
        showFrontButton.onClick.AddListener(ShowFront);
    }

    private void Like() {
        ratingSystem.Like();
    }

    private void Dislike() {
        ratingSystem.Dislike();
    }

    private void ShowBack() {
        showBackButton.gameObject.SetActive(false);
        showFrontButton.gameObject.SetActive(true);
        //ratingSystem.currentCanvas.SetCanvasTexture(backTexture);
    }

    private void ShowFront() {
        showBackButton.gameObject.SetActive(true);
        showFrontButton.gameObject.SetActive(false);
        //ratingSystem.currentCanvas.SetCanvasTexture(frontTexture);
    }

    private Coroutine moveRoutine;
    public void Move(bool moveIn) {
        if (moveIn) {
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
