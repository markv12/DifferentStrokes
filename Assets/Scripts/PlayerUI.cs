using UnityEngine;

public class PlayerUI : MonoBehaviour {
    public GameObject drawIndicator;

    private void Awake() {
        drawIndicator.SetActive(false);
    }

    public void RefreshForCanvas(PaintingCanvas currentCanvas) {
        if(currentCanvas != null) {
            drawIndicator.SetActive(true);
        } else {
            drawIndicator.SetActive(false);
        }
    }
}
