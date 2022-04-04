using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour {
    public TMP_Text speechText;
    public RectTransform containerT;
    public RectTransform backgroundT;
    private Vector3 initialScale;

    private void Awake() {
        initialScale = backgroundT.localScale;
        backgroundT.localScale = Vector3.zero;
    }

    public void SetText(string text) {
        speechText.text = text;
        backgroundT.sizeDelta = new Vector2(speechText.preferredWidth + 10, speechText.preferredHeight + 10);
    }

    public void Fade(bool fadeIn, bool facingFront) {
        if (fadeIn) {
            containerT.localEulerAngles = new Vector3(0, facingFront ? 0 : 180, 0);
        }
        gameObject.SetActive(true);
        Vector3 startScale = backgroundT.localScale;
        Vector3 endScale = fadeIn ? initialScale : Vector3.zero;
        this.CreateAnimationRoutine(0.4f, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            backgroundT.localScale = Vector3.Lerp(startScale, endScale, easedProgress);
        }, () => {
            if (!fadeIn) {
                gameObject.SetActive(false);
            }
        });
    }
}
