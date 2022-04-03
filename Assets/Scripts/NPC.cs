using System;
using System.Collections;
using UnityEngine;

public class NPC : InteractiveObject {

    [TextArea(4, 12)]
    public string dialogueText;
    [Range(0.1f, 5f)]
    public float pitchCenter = 1;
    [TextArea(2, 3)]
    public string[] speechBubbleTexts;
    [NonSerialized] public SpeechBubble speechBubble;
    public override string InteractText => "Press 'E' to Talk";
    public override bool Interactable => !string.IsNullOrWhiteSpace(dialogueText);
    public override void OnNearChanged(bool isNear) {
        if(isNear && speechBubbleTexts.Length > 0) {
            string text = speechBubbleTexts[UnityEngine.Random.Range(0, speechBubbleTexts.Length)];
            if (!string.IsNullOrWhiteSpace(text)) {
                SetSpeechBubbleActive(true);
                speechBubble.SetText(text);
            } else {
                SetSpeechBubbleActive(false);
            }
        } else {
            SetSpeechBubbleActive(false);
        }
    }

    private void SetSpeechBubbleActive(bool active) {
        if (active) {
            if(speechBubble == null) {
                speechBubble = ResourceManager.InstantiatePrefab<SpeechBubble>("SpeechBubble", Vector3.zero);
                speechBubble.transform.SetParent(transform, false);
                speechBubble.transform.localPosition = new Vector3(0.25f, 0.25f, 0);
            }
            speechBubble.Fade(true);
        } else {
            if(speechBubble != null) {
                speechBubble.Fade(false);
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
        float waitTime = UnityEngine.Random.Range(0f, timePerFrame);
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
