using System.Collections.Generic;
using UnityEngine;

public class PaintingCanvasManager : Singleton<PaintingCanvasManager> {
    private static readonly List<PaintingCanvas> activeCanvases = new List<PaintingCanvas>();

    private const float MAX_SQUARE_DISTANCE = 20f;
    public PaintingCanvas GetNearestCanvas(Vector3 pos) {
        PaintingCanvas result = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < activeCanvases.Count; i++) {
            PaintingCanvas paintingCanvas = activeCanvases[i];
            float sqDistance = (pos - paintingCanvas.pos).sqrMagnitude;
            if(sqDistance < MAX_SQUARE_DISTANCE && sqDistance < minDistance) {
                result = paintingCanvas;
                minDistance = sqDistance;
            }
        }
        return result;
    }

    public void RegisterCanvas(PaintingCanvas paintingCanvas) {
        if (!activeCanvases.Contains(paintingCanvas)) {
            activeCanvases.Add(paintingCanvas);
        }
    }

    public void UnregisterCanvas(PaintingCanvas paintingCanvas) {
        activeCanvases.Remove(paintingCanvas);
    }
}
