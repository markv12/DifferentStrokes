using System.Collections;
using UnityEngine;

public class NPC : InteractiveObject {

    [TextArea(4, 8)]
    public string dialogueText;
    [Range(0.1f, 5f)]
    public float pitchCenter = 1;
    public override string InteractText => "Press 'E' to Talk";
    public override bool Interactable => !string.IsNullOrWhiteSpace(dialogueText);

    public SpriteRenderer mainRenderer;
    public Sprite[] sprites;
    public float frameRate;

    private Coroutine animateRoutine = null;
    private void OnEnable() {
        OnDisable();
        animateRoutine = StartCoroutine(Co_Animate());
    }

    private void OnDisable() {
        this.EnsureCoroutineStopped(ref animateRoutine);
    }

    IEnumerator Co_Animate() {
        yield return null;
        yield return null;
        yield return null;
        float timePerFrame = 1f / frameRate;
        float waitTime = Random.Range(0f, timePerFrame);
        yield return new WaitForSeconds(waitTime);

        while (true) {
            for (int i = 0; i < sprites.Length; i++) {
                mainRenderer.sprite = sprites[i];
                float elapsedTime = 0;
                while (elapsedTime < timePerFrame) {
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }
    }
}
