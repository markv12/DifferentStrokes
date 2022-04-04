using UnityEngine;

public class ScaleFixer : MonoBehaviour {
    public MeshRenderer mainRenderer;
    private void Awake() {
        Vector4 worldScale = transform.localScale;
        worldScale.w = 1;
        mainRenderer.material.SetVector("_WorldScale", worldScale);
    }
}
