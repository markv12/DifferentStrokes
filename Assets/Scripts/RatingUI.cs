using UnityEngine;
using UnityEngine.UI;

public class RatingUI : MonoBehaviour {
    public RatingSystem ratingSystem;
    public RectTransform mainContainer;
    public Button likeButton;
    public Button dislikeButton;
    public Button exitButton;
    public Button showBackButton;
    public Button showFrontButton;

    private void Awake() {
        likeButton.onClick.AddListener(Like);
        dislikeButton.onClick.AddListener(Dislike);
        exitButton.onClick.AddListener(Exit);
        showBackButton.onClick.AddListener(ShowBack);
        showFrontButton.onClick.AddListener(ShowFront);
    }

    private void Like() {
        AudioManager.Instance.PlaySuccessSound(0.3f);
        ratingSystem.Like();
    }

    private void Dislike() {
        AudioManager.Instance.PlayFailureSound(0.3f);
        ratingSystem.Dislike();
    }

    private void Exit() {
        ratingSystem.Cancel();
    }

    private void ShowBack() {
        AudioManager.Instance.PlayPaperFlip(0.5f);
        likeButton.gameObject.SetActive(false);
        dislikeButton.gameObject.SetActive(false);
        showBackButton.gameObject.SetActive(false);
        showFrontButton.gameObject.SetActive(true);
        ratingSystem.ShowOriginal();
    }

    private void ShowFront() {
        AudioManager.Instance.PlayPaperFlip(0.5f);
        likeButton.gameObject.SetActive(ratingSystem.currentCanvas.PaintingStatus == PaintingStatus.Complete);
        dislikeButton.gameObject.SetActive(ratingSystem.currentCanvas.PaintingStatus == PaintingStatus.Complete);
        showBackButton.gameObject.SetActive(true);
        showFrontButton.gameObject.SetActive(false);
        ratingSystem.ShowCompleted();
    }

    private Coroutine moveRoutine;
    public void Move(bool moveIn) {
        if (moveIn) {
            ShowFront();
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
