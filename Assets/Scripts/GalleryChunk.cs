using UnityEngine;

public class GalleryChunk : MonoBehaviour {
    public Transform t;
    public Transform[] paintingSpawnLocations;

    public void MoveToPosition(Vector3 endPos) {
        Vector3 startPos = endPos.SetY(endPos.y - 10f);
        t.position = startPos;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            float easedProgress = Easing.easeOutCubic(0f, 1f, progress);
            t.position = Vector3.Lerp(startPos, endPos, easedProgress);
        });
    }
}
