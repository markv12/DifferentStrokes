using UnityEngine;

public class PaintingCanvas : InteractiveObject {
    public override string InteractText {
        get {
            switch (paintingStatus) {
                case PaintingStatus.Complete:
                    return "Press 'E' to Rate";
                case PaintingStatus.HallOfFame:
                    return "Press 'E' to View";
                default:
                    return "Press 'E' to Paint";
            }
        }
    }
    public override bool Interactable => !locked;
    public override bool MustBeFacing => true;
    public override void OnNearChanged(bool isNear) {}

    public SpriteRenderer canvasSpriteRenderer;

    public SpriteRenderer frameRenderer;
    public Sprite blankFrame;
    public Sprite needsFixingFrame;
    public Sprite completeFrame;

    public GameObject paintMeSign;
    public GameObject fixThisSign;
    public GameObject barrier;

    private Texture2D mainTex;
    private Texture2D originalTex;

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

    private bool locked = false;
    public bool Locked {
        get {
            return locked;
        }
        set {
            locked = value;
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
        mainTex = tex;
        CreateAndSetSprite(tex);
    }

    private void CreateAndSetSprite(Texture2D tex) {
        canvasSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
    }

    public void SetOriginalTexture(Texture2D tex) {
        originalTex = tex;
    }

    public void ShowOriginalTexture() {
        if(originalTex != null) {
            CreateAndSetSprite(originalTex);
        }
    }

    public void ShowCompleteTexture() {
        if(mainTex != null) {
            CreateAndSetSprite(mainTex);
        }
    }

    public Texture2D GetCanvasTexture() {
        return canvasSpriteRenderer.sprite.texture;
    }

    private void RefreshForPaintingStatus() {
        frameRenderer.sprite = GetFrameForStatus();
        paintMeSign.SetActive(PaintingStatus == PaintingStatus.Blank && !locked);
        fixThisSign.SetActive(PaintingStatus == PaintingStatus.NeedsFixing && !locked);
        barrier.SetActive(PaintingStatus == PaintingStatus.Complete || PaintingStatus == PaintingStatus.HallOfFame || locked);
    }

    private Sprite GetFrameForStatus() {
        switch (PaintingStatus) {
            case PaintingStatus.Blank:
                return blankFrame;
            case PaintingStatus.NeedsFixing:
                return needsFixingFrame;
            case PaintingStatus.Complete:
            case PaintingStatus.HallOfFame:
                return completeFrame;
            default:
                return completeFrame;
        }
    }
}
public enum PaintingStatus {
    Blank,
    NeedsFixing,
    Complete,
    HallOfFame,
}
