using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

// unused

[BurstCompile]
public struct VoxelDataPhysicsJob : IJob {
    public NativeArray<bool> successful;
    [NativeDisableParallelForRestriction] public NativeArray<float> voxelData;
    [NativeDisableParallelForRestriction] public NativeArray<Vector2Int> IDs;

    public float isoLevel;
    public int xz, y;

    public VoxelDataPhysicsJob(NativeArray<bool> successful, NativeArray<float> voxelData, NativeArray<Vector2Int> IDs, float isoLevel, int xz, int y) {
        this.successful = successful;
        this.voxelData = voxelData;
        this.IDs = IDs;
        this.isoLevel = isoLevel;
        this.xz = xz;
        this.y = y;
    }

    void IJob.Execute() {
        bool anyChanged = false;

        for (int x = 0; x < xz; x ++) {
            for (int y = 0; y < this.y; y ++) {
                for (int z = 0; z < xz; z ++) {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    if (IterateAtPos(pos)) {
                        anyChanged = true;
                    }
                }
            }
        }

        successful[0] = anyChanged;
    }

    bool IterateAtPos(Vector3Int pos) {
        Vector3Int posDown = pos + Vector3Int.down;

        int index = IndexUtilities.Index3D(pos, xz, y);

        if (index != -1) {
            float data = voxelData[index];

            if (data < isoLevel) {
                int indexDown = IndexUtilities.Index3D(posDown, xz, y);

                if (indexDown != -1) {
                    float dataDown = voxelData[indexDown];

                    if (dataDown >= isoLevel) {
                        voxelData[indexDown] = data;
                        voxelData[index] = dataDown;

                        return true;
                    }
                }
            }
        }

        return false;
    }
}