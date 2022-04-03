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
        AudioManager.Instance.PlaySuccessSound(0.3f);
        ratingSystem.Like();
    }

    private void Dislike() {
        AudioManager.Instance.PlayFailureSound(0.3f);
        ratingSystem.Dislike();
    }

    private void ShowBack() {
        likeButton.gameObject.SetActive(false);
        dislikeButton.gameObject.SetActive(false);
        showBackButton.gameObject.SetActive(false);
        showFrontButton.gameObject.SetActive(true);
        ratingSystem.ShowOriginal();
    }

    private void ShowFront() {
        likeButton.gameObject.SetActive(true);
        dislikeButton.gameObject.SetActive(true);
        showBackButton.gameObject.SetActive(true);
        showFrontButton.gameObject.SetActive(false);
        ratingSystem.ShowCompleted();
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
