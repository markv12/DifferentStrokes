using System.Collections;
using UnityEngine;

public class NPC : InteractiveObject {

    [TextArea(4, 8)]
    public string dialogueText;
    [Range(0.1f, 5f)]
    public float pitchCenter = 1;
    [TextArea(4, 8)]
    public string speechBubbleText;
    public SpeechBubble speechBubble;
    public override string InteractText => "Press 'E' to Talk";
    public override bool Interactable => !string.IsNullOrWhiteSpace(dialogueText);
    public override void OnNearChanged(bool isNear) {
        if(isNear && !string.IsNullOrWhiteSpace(speechBubbleText)) {
            SetSpeechBubbleActive(true);
            speechBubble.SetText(speechBubbleText);
        } else {
            SetSpeechBubbleActive(false);
        }
    }

    private void SetSpeechBubbleActive(bool active) {
        if (active) {
            if(speechBubble == null) {
                speechBubble = ResourceManager.InstantiatePrefab<SpeechBubble>("SpeechBubble", Vector3.zero);
                speechBubble.transform.SetParent(transform, false);
                speechBubble.transform.localPosition = new Vector3(1, 1, 0);
            }
            speechBubble.gameObject.SetActive(true);
        } else {
            if(speechBubble != null) {
                speechBubble.gameObject.SetActive(false);
            }
        }
    }

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
