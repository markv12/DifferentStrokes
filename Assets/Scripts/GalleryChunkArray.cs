using UnityEngine;

public class GalleryChunkArray {
    private const int DIMENSION = 400;
    private const int HALF_DIMENSION = DIMENSION/2;
    private readonly GalleryChunk[,] chunks = new GalleryChunk[DIMENSION, DIMENSION];

    public GalleryChunk Get(Vector2Int index) {
        return chunks[index.x + HALF_DIMENSION, index.y + HALF_DIMENSION];
    }

    public GalleryChunk Get(int x, int z) {
        return chunks[x + HALF_DIMENSION, z + HALF_DIMENSION];
    }

    public void Set(Vector2Int index, GalleryChunk chunk) {
        chunks[index.x + HALF_DIMENSION, index.y + HALF_DIMENSION] = chunk;
    }

    public void Set(int x, int z, GalleryChunk chunk) {
        chunks[x + HALF_DIMENSION, z + HALF_DIMENSION] = chunk;
    }
}

