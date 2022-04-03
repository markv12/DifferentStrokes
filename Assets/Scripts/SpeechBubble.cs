using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour {
    public TMP_Text speechText;
    public RectTransform rectT;
    private Vector3 initialScale;

    private void Awake() {
        initialScale = rectT.localScale;
        rectT.localScale = Vector3.zero;
    }

    public void SetText(string text) {
        speechText.text = text;
    }

    public void Fade(bool fadeIn) {
        if (fadeIn) {
            gameObject.SetActive(true);
        }
        Vector3 startScale = rectT.localScale;
        Vector3 endScale = fadeIn ? initialScale : Vector3.zero;
        this.CreateAnimationRoutine(0.4f, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            rectT.localScale = Vector3.Lerp(startScale, endScale, easedProgress);
        }, () => {
            if (!fadeIn) {
                gameObject.SetActive(false);
            }
        });
    }
}
