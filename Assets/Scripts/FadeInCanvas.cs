using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInCanvas : MonoBehaviour {
    public Image mainImage;

    private IEnumerator Start() {
        Color startColor = mainImage.color;
        yield return new WaitForSeconds(0.75f);
        this.CreateAnimationRoutine(1.25f, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainImage.color = Color.Lerp(startColor, ColorExtensions.WHITE_CLEAR, easedProgress);
        }, () => {
            Destroy(gameObject);
        });
    }
}