using System.Collections;
using UnityEngine;

public class HallOfFameManager : MonoBehaviour {

    public Transform[] spawnLocations;

    private void Start() {
        StartCoroutine(Load());

        IEnumerator Load() {
            while (ArtManager.instance == null || !ArtManager.instance.Initialized) {
                yield return null;
            }
            ArtManager.instance.AddPaintingsForHallOfFame(spawnLocations);
        }
    }
}
