using System;
using UnityEngine;

public class PaintingCanvas : MonoBehaviour {
    public MeshRenderer canvasMeshRenderer;

    [NonSerialized] public Vector3 pos;
    [NonSerialized] public PaintingStatus paintingStatus;

    private void Awake() {
        pos = transform.position;
        PaintingCanvasManager.Instance.RegisterCanvas(this);
    }

    public void SetCanvasTexture(Texture tex) {
        canvasMeshRenderer.material.SetTexture("_MainTex", tex);
    }

    private void OnDestroy() {
        if(PaintingCanvasManager.Instance != null) {
            PaintingCanvasManager.Instance.UnregisterCanvas(this);
        }
    }
}
public enum PaintingStatus {
    Blank,
    NeedsFixing,
    Complete
}
