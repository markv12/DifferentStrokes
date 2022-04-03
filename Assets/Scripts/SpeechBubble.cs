using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TMP_Text speechText;

    public void SetText(string text) {
        speechText.text = text;
    }
}
