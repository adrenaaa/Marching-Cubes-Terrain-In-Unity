using UnityEngine;
using Unity.Collections;

public struct VoxelData {
    public NativeArray<float> voxelData;
    public NativeArray<Vector2Int> voxelDataIDs;

    public readonly int xz;
    public readonly int y;

    public readonly Vector3Int chunkPos;

    public VoxelData(int sizeXZ, int sizeY, Vector3Int chunkPos) {
        voxelData = new NativeArray<float>(sizeXZ * sizeY * sizeXZ, Allocator.Persistent);
        voxelDataIDs = new NativeArray<Vector2Int>(sizeXZ * sizeY * sizeXZ, Allocator.Persistent);

        xz = sizeXZ;
        y = sizeY;

        this.chunkPos = chunkPos;
    }

    public void SetVoxelDataDirect(int index, float data) {
        if (index < 0 || index >= voxelData.Length) {
            return;
        }

        voxelData[index] = data;
    }

    public void SetVoxelDataIDDirect(int index, Vector2Int ID) {
        if (index < 0 || index >= voxelDataIDs.Length) {
            return;
        }

        voxelDataIDs[index] = ID;
    }

    public bool TryGetVoxelData(int index, out float data) {
        if (index < 0 || index >= voxelData.Length) {
            data = -1f;

            return false;
        }

        data = voxelData[index];

        return true;
    }

    public bool TryGetVoxelDataID(int index, out Vector2Int ID) {
        if (index < 0 || index >= voxelDataIDs.Length) {
            ID = -Vector2Int.one;

            return false;
        }

        ID = voxelDataIDs[index];

        return true;
    }

    public Cube CalculateCube(Vector3Int pos) {
        Cube cube = new Cube();

        for (int i = 0; i < 8; i ++) {
            Vector3Int samplePos = pos + MeshCore.corners[i];
            int index = IndexUtilities.Index3D(samplePos, xz, y);

            TryGetVoxelData(index, out float data);

            cube.SetCorner(i, data);
        }

        return cube;
    }

    public void StoreVoxelData(VoxelDataStorage voxelDataStorage) {
        voxelDataStorage.StoreVoxelData(chunkPos, voxelData.ToArray());

        DisposeVoxelData();
    }

    public void StoreVoxelDataIDs(VoxelDataStorage voxelDataStorage) {
        voxelDataStorage.StoreVoxelDataIDs(chunkPos, voxelDataIDs.ToArray());

        DisposeVoxelDataIDs();
    }

    public void GetVoxelDataFromStorage(VoxelDataStorage voxelDataStorage) {
        if (!voxelDataStorage.TryGetAllVoxelData(chunkPos, true, out float[] extractedVoxelData)) {
            throw new System.IndexOutOfRangeException($"Chunk position \"{chunkPos}\" does not exist in storage.");
        }

        voxelData = new NativeArray<float>(extractedVoxelData.Length, Allocator.Persistent);

        for (int i = 0; i < voxelData.Length; i ++) {
            voxelData[i] = extractedVoxelData[i];
        }
    }

    public void GetVoxelDataIDsFromStorage(VoxelDataStorage voxelDataStorage) {
        if (!voxelDataStorage.TryGetAllVoxelDataIDs(chunkPos, true, out Vector2Int[] extractedVoxelDataIDs)) {
            throw new System.IndexOutOfRangeException($"Chunk position \"{chunkPos}\" does not exist in storage.");
        }

        voxelDataIDs = new NativeArray<Vector2Int>(extractedVoxelDataIDs.Length, Allocator.Persistent);

        for (int i = 0; i < voxelDataIDs.Length; i ++) {
            voxelDataIDs[i] = extractedVoxelDataIDs[i];
        }
    }

    public void DisposeVoxelData() {
        voxelData.Dispose();
    }

    public void DisposeVoxelDataIDs() {
        voxelDataIDs.Dispose();
    }
}