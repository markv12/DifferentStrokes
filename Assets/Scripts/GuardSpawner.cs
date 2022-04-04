using UnityEngine;

public class GuardSpawner : MonoBehaviour {
    public NPC[] guardPrefabs;
    public static GuardSpawner instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnGuardsAtSpawnLocations(Transform[] spawnLocations) {
        for (int i = 0; i < spawnLocations.Length; i++) {
            Transform spawnLocation = spawnLocations[i];
            if(spawnLocation != null && Random.value > 0.5f) {
                NPC guardPrefab = guardPrefabs[Random.Range(0, guardPrefabs.Length)];
                Instantiate(guardPrefab, spawnLocation);
            }
        }
    }
}
