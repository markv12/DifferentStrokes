using System;
using UnityEngine;

public class PaintingCanvas : InteractiveObject {
    public override string InteractText => "Press 'E' to Draw";

    public SpriteRenderer canvasSpriteRenderer;

    [NonSerialized] public PaintingStatus paintingStatus;

    public void SetCanvasTexture(RenderTexture rTex) {
        RenderTexture prevActiveRT = RenderTexture.active;
        RenderTexture.active = rTex;
        Texture2D tex = new Texture2D(rTex.width, rTex.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prevActiveRT;
        canvasSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
    }
}
public enum PaintingStatus {
    Blank,
    NeedsFixing,
    Complete
}
