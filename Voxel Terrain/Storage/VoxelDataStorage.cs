using UnityEngine;
using System.Collections.Generic;

public class VoxelDataStorage : MonoBehaviour {
    Dictionary<Vector3Int, float[]> storedVoxelData = new Dictionary<Vector3Int, float[]>();
    Dictionary<Vector3Int, Vector2Int[]> storedVoxelDataIDs = new Dictionary<Vector3Int, Vector2Int[]>();

    public void StoreVoxelData(Vector3Int chunkPos, float[] voxelData) {
        storedVoxelData.Add(chunkPos, voxelData);
    }

    public void StoreVoxelDataIDs(Vector3Int chunkPos, Vector2Int[] voxelDataIDs) {
        storedVoxelDataIDs.Add(chunkPos, voxelDataIDs);
    }

    public bool TryGetAllVoxelData(Vector3Int chunkPos, bool removeFromStorage, out float[] voxelData) {
        bool successful = false;
        voxelData = new float[0];

        if (storedVoxelData.TryGetValue(chunkPos, out voxelData)) {
            successful = true;

            if (removeFromStorage) {
                RemoveVoxelData(chunkPos);
            }
        }

        return successful;
    }

    public bool TryGetAllVoxelDataIDs(Vector3Int chunkPos, bool removeFromStorage, out Vector2Int[] voxelDataIDs) {
        bool successful = false;
        voxelDataIDs = new Vector2Int[0];

        if (storedVoxelDataIDs.TryGetValue(chunkPos, out voxelDataIDs)) {
            successful = true;

            if (removeFromStorage) {
                RemoveVoxelDataIDs(chunkPos);
            }
        }

        return successful;
    }

    public bool TryGetVoxelData(Vector3Int chunkPos, int index, out float voxelData) {
        bool successful = false;
        voxelData = -1f;

        if (storedVoxelData.TryGetValue(chunkPos, out float[] allVoxelData)) {
            if (index < 0 || index >= allVoxelData.Length) {
                return false;
            }

            voxelData = allVoxelData[index];
            
            successful = true;
        }

        return successful;
    }

    public bool TryGetVoxelDataID(Vector3Int chunkPos, int index, out Vector2Int voxelDataID) {
        bool successful = false;
        voxelDataID = -Vector2Int.one;

        if (storedVoxelDataIDs.TryGetValue(chunkPos, out Vector2Int[] voxelDataIDs)) {
            if (index < 0 || index >= voxelDataIDs.Length) {
                return false;
            }

            voxelDataID = voxelDataIDs[index];
            
            successful = true;
        }

        return successful;
    }

    public bool TryModifyVoxelDataDirect(Vector3Int chunkPos, int index, float value, bool add) {
        if (!storedVoxelData.ContainsKey(chunkPos) || index == -1) {
            return false;
        }

        if (add) {
            storedVoxelData[chunkPos][index] += value;
        }
        else {
            storedVoxelData[chunkPos][index] = value;
        }

        return true;
    }

    public bool TryModifyVoxelDataIDsDirect(Vector3Int chunkPos, int index, Vector2Int ID) {
        if (!storedVoxelDataIDs.ContainsKey(chunkPos) || index == -1) {
            return false;
        }

        storedVoxelDataIDs[chunkPos][index] = ID;

        return true;
    }

    public void RemoveVoxelData(Vector3Int chunkPos) {
        storedVoxelData.Remove(chunkPos);
    }

    public void RemoveVoxelDataIDs(Vector3Int chunkPos) {
        storedVoxelDataIDs.Remove(chunkPos);
    }
}