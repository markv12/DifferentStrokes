using System;
using UnityEngine;

public class PaintingCanvas : InteractiveObject {
    public override string InteractText => "Press 'E' to Draw";
    public override bool Interactable => PaintingStatus != PaintingStatus.Complete;

    public SpriteRenderer canvasSpriteRenderer;

    public SpriteRenderer frameRenderer;
    public Sprite blankFrame;
    public Sprite needsFixingFrame;
    public Sprite completeFrame;

    public GameObject barrier;

    private PaintingStatus paintingStatus;
    public PaintingStatus PaintingStatus {
        get {
            return paintingStatus;
        }
        set {
            paintingStatus = value;
            RefreshForPaintingStatus();
        }
    }

    public string ImageID {
        get; set;
    }

    public void SetCanvasTexture(RenderTexture rTex) {
        RenderTexture prevActiveRT = RenderTexture.active;
        RenderTexture.active = rTex;
        Texture2D tex = new Texture2D(rTex.width, rTex.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prevActiveRT;
        SetCanvasTexture(tex);
    }

    public void SetCanvasTexture(Texture2D tex) {
        canvasSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
    }

    private void RefreshForPaintingStatus() {
        frameRenderer.sprite = GetFrameForStatus();
        barrier.SetActive(PaintingStatus == PaintingStatus.Complete);
    }

    private Sprite GetFrameForStatus() {
        switch (PaintingStatus) {
            case PaintingStatus.Blank:
                return blankFrame;
            case PaintingStatus.NeedsFixing:
                return needsFixingFrame;
            case PaintingStatus.Complete:
                return completeFrame;
            default:
                return completeFrame;
        }
    }
}
public enum PaintingStatus {
    Blank,
    NeedsFixing,
    Complete
}
