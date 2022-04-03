using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour {
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    private Action onExit;
    private Transform mainCameraTransform;
    private Vector3 mainCameraOriginalPos;
    private Quaternion mainCameraOriginalRotation;

    private NPC currentNPC;

    private DialogueVertexAnimator dialogueVertexAnimator;

    private bool inDialogueMode = false;
    private bool InDialogueMode {
        get {
            return inDialogueMode;
        }
        set {
            inDialogueMode = value;
            dialogueUI.SetActive(inDialogueMode);
        }
    }

    public void TalkToNPC(NPC npc, Camera _mainCamera, Action _onExit) {
        currentNPC = npc;
        mainCameraTransform = _mainCamera.transform;
        onExit = _onExit;

        mainCameraOriginalPos = mainCameraTransform.localPosition;
        mainCameraOriginalRotation = mainCameraTransform.localRotation;

        MoveCameraInFrontOfCanvas();
    }

    private void Awake() {
        dialogueVertexAnimator = new DialogueVertexAnimator(dialogueText, null, null);
    }

    private void Update() {
        if (InDialogueMode) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ReturnCameraToPlayer();
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) {
                if (dialogueVertexAnimator.textAnimating) {
                    dialogueVertexAnimator.SkipToEndOfCurrentMessage();
                } else {
                    if (currentTextLine < textLines.Length) {
                        AnimateNextTextLine();
                    } else {
                        ReturnCameraToPlayer();
                    }
                }
            }
        }
    }

    private void MoveCameraInFrontOfCanvas() {
        Vector3 startPos = mainCameraTransform.position;
        Quaternion startRotation = mainCameraTransform.rotation;
        Vector3 forwardVector = GetForwardVector();
        Vector3 endPos = currentNPC.transform.position + (forwardVector * -2.05f);
        Quaternion endRotation = Quaternion.LookRotation(forwardVector, currentNPC.transform.up);
        this.CreateAnimationRoutine(DrawingSystem.ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, easedProgress), Quaternion.Lerp(startRotation, endRotation, easedProgress));
        }, () => {
            InDialogueMode = true;
            StartDialogue();
        });
    }

    private Vector3 GetForwardVector() {
        Vector3 npcForward = currentNPC.transform.forward;
        bool isFacingForward = Vector3.Dot(mainCameraTransform.forward, npcForward) > 0;
        return isFacingForward ? npcForward : -npcForward;
    }

    private Coroutine typeRoutine = null;
    private int currentTextLine = 0;
    private string[] textLines;
    public static readonly string[] DOUBLE_NEW_LINE = new string[] { "\n\n" };
    private void StartDialogue() {
        dialogueText.text = "";
        textLines = currentNPC.dialogueText.Split(DOUBLE_NEW_LINE, StringSplitOptions.None);
        currentTextLine = 0;
        AnimateNextTextLine();
    }

    private void AnimateNextTextLine() {
        StopTyping();
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(textLines[currentTextLine], out string processedMessage);
        currentTextLine++;
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, processedMessage, null, currentNPC.pitchCenter, null));
    }

    public void StopTyping() {
        this.EnsureCoroutineStopped(ref typeRoutine);
    }

    public void ReturnCameraToPlayer() {
        InDialogueMode = false;
        Vector3 startPos = mainCameraTransform.localPosition;
        Quaternion startRotation = mainCameraTransform.localRotation;
        Vector3 endPos = mainCameraOriginalPos;
        this.CreateAnimationRoutine(DrawingSystem.ANIM_DURATION, (float progress) => {
            float easedProgress = Easing.easeInOutSine(0f, 1f, progress);
            mainCameraTransform.localPosition = Vector3.Lerp(startPos, mainCameraOriginalPos, easedProgress);
            mainCameraTransform.localRotation = Quaternion.Lerp(startRotation, mainCameraOriginalRotation, easedProgress);
        }, () => {
            onExit?.Invoke();
        });
    }
}
