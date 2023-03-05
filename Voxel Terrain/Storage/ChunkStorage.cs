using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChunkStorage : MonoBehaviour {
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    public void StoreChunk(Vector3Int chunkPos, Chunk chunk) {
        chunks.Add(chunkPos, chunk);
    }

    public bool TryGetChunk(Vector3Int chunkPos, out Chunk chunk) {
        if (!chunks.ContainsKey(chunkPos)) {
            chunk = null;

            return false;
        }

        chunk = chunks[chunkPos];

        return true;
    }

    public bool ContainsChunk(Vector3Int chunkPos) {
        return chunks.ContainsKey(chunkPos);
    }

    public bool TrySetChunkActive(Vector3Int chunkPos, bool active) {
        if (!chunks.TryGetValue(chunkPos, out Chunk chunk)) {
            return false;
        }

        chunk.SetChunkActive(active);

        return true;
    }

    public Vector3Int[] GetChunkPossAroundPoint(Vector3 point) {
        List<Vector3Int> chunkPoss = new List<Vector3Int>();

        chunkPoss.Add(GetChunkPosFromPoint(point));

        for (int i = 0; i < 8; i ++) {
            Vector3 samplePoint = point + MeshCore.cardinalDirections[i];
            Vector3Int chunkPos = GetChunkPosFromPoint(samplePoint);
            
            if (ContainsChunk(chunkPos)) {
                chunkPoss.Add(chunkPos);
            }
        }

        return chunkPoss.Distinct().ToArray();
    }

    public Vector3Int GetChunkPosFromPoint(Vector3 point) {
        float x = point.x;
        float z = point.z;

        if (point.x == Mathf.FloorToInt(point.x)) {
            x -= 0.1f;
        }

        if (point.z == Mathf.FloorToInt(point.z)) {
            z -= 0.1f;
        }

        Vector3Int chunkPos = new Vector3Int (
            Mathf.FloorToInt(x / World.sizeXZ) * World.sizeXZ,
            0,
            Mathf.FloorToInt(z / World.sizeXZ) * World.sizeXZ
        );

        return chunkPos;
    }
}