using System.Collections;
using UnityEngine;

public class GalleryChunk : MonoBehaviour {
    public Transform t;
    public Transform[] paintingSpawnLocations;
    public Transform[] ghostSpawnLocations;

    public void MoveToPosition(Vector3 endPos) {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();

        Vector3 startPos = endPos.SetY(endPos.y - 10f);
        t.position = startPos;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            float easedProgress = Easing.easeOutCubic(0f, 1f, progress);
            t.position = Vector3.Lerp(startPos, endPos, easedProgress);
        });
    }

    private void Start() {
        StartCoroutine(Load());

        IEnumerator Load() {
            while(ArtManager.instance == null || !ArtManager.instance.Initialized) {
                yield return null;
            }
            ArtManager.instance.AddPaintingsForChunk(paintingSpawnLocations);
        }
    }
}
