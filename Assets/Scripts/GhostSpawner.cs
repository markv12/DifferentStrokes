using UnityEngine;

public class GhostSpawner : MonoBehaviour {
    public NPC[] ghostPrefabs;
    public static GhostSpawner instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnGhostsAtSpawnLocations(Transform[] spawnLocations) {
        for (int i = 0; i < spawnLocations.Length; i++) {
            Transform spawnLocation = spawnLocations[i];
            if(spawnLocation != null) {
                NPC ghostPrefab = ghostPrefabs[Random.Range(0, ghostPrefabs.Length)];
                Instantiate(ghostPrefab, spawnLocation);
            }
        }
    }
}
