using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct VoxelJob : IJob {
    [NativeDisableParallelForRestriction] public NativeList<VertexData> vertexData;

    public VoxelData voxelData;

    public float isoLevel;

    public VoxelJob(NativeList<VertexData> vertexData, VoxelData voxelData, float isoLevel) {
        this.vertexData = vertexData;
        this.voxelData = voxelData;
        this.isoLevel = isoLevel;
    }

    void IJob.Execute() {
        for (int x = 0; x < voxelData.xz; x ++) {
            for (int y = 0; y < voxelData.y; y ++) {
                for (int z = 0; z < voxelData.xz; z ++) {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    Voxelise(pos);
                }
            }
        }
    }

    void Voxelise(Vector3Int pos) {
        int index = IndexUtilities.Index3D(pos, voxelData.xz, voxelData.y);
        voxelData.TryGetVoxelData(index, out float data);

        if (data >= isoLevel) {
            return;
        }

        IndiceList indiceList = GetIndicesFromNeighbours(pos);

        for (int i = 0; i < 6; i ++) {
            int indice = indiceList.GetIndice(i);

            if (indice == -1) {
                continue;
            }

            int rowIndex = indice * 6;

            for (int j = 0; j < 6; j ++) {
                int triIndex = rowIndex + j;

                Vector3 vertPos = pos + MeshCore.corners[VoxelCore.triTable[triIndex]];
                voxelData.TryGetVoxelDataID(index, out Vector2Int ID);
                
                VertexData vertexData = new VertexData(vertPos, ID);

                this.vertexData.Add(vertexData);
            }
        }
    }

    IndiceList GetIndicesFromNeighbours(Vector3Int pos) {
        IndiceList indices = new IndiceList();

        for (int i = 0; i < 6; i ++) {
            Vector3Int samplePos = pos + VoxelCore.neighbourSampleDirections[i];

            int index = IndexUtilities.Index3D(samplePos, voxelData.xz, voxelData.y);
            int indice = -1;

            if (voxelData.TryGetVoxelData(index, out float data)) {
                if (data >= isoLevel) {
                    indice = i;
                }
            }

            indices.SetIndice(i, indice);
        }

        return indices;
    }
}