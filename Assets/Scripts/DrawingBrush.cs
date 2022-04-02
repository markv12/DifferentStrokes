using UnityEngine;

public class DrawingBrush : MonoBehaviour
{
    public MeshRenderer mainRenderer;

    public void SetColor(Color c) {
        mainRenderer.material.SetColor("_Color", c);
    }
}
