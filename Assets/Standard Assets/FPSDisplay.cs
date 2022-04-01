using UnityEngine;

public class FPSDisplay : MonoBehaviour {
    float deltaTime = 0.0f;

    private static GUIStyle style = new GUIStyle();
    private static Rect rect;

    void Awake() {
        int w = Screen.width, h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 80);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 80;
        style.normal.textColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
    }

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.01f;
    }

    void OnGUI() {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format(" {0:0.0} ms | {1:0.} FPS", msec, fps);
        GUI.Label(rect, text, style);
    }
}
