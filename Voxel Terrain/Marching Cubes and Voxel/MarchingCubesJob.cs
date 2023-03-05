using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct MarchingCubesJob : IJob {
    [NativeDisableParallelForRestriction] public NativeList<VertexData> vertexData;

    public VoxelData voxelData;

    public float isoLevel;

    public bool midpoints;

    public MarchingCubesJob(NativeList<VertexData> vertexData, VoxelData voxelData, float isoLevel, bool midpoints) {
        this.vertexData = vertexData;
        this.voxelData = voxelData;
        this.isoLevel = isoLevel;
        this.midpoints = midpoints;
    }

    void IJob.Execute() {
        for (int x = 0; x < voxelData.xz - 1; x ++) {
            for (int y = 0; y < voxelData.y - 1; y ++) {
                for (int z = 0; z < voxelData.xz - 1; z ++) {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    MarchCube(pos);
                }
            }
        }
    }

    void MarchCube(Vector3Int pos) {
        Cube cube = voxelData.CalculateCube(pos);

        int cubeIndex = MarchingCubesCore.CalculateCubeIndex(cube, isoLevel);

        if (cubeIndex == 0 || cubeIndex == 255) {
            return;
        }

        VertexList vertexList = MarchingCubesCore.CalculateVertexList(pos, cube, isoLevel, cubeIndex, midpoints);

        int rowIndex = cubeIndex * 16;

        for (int i = 0; MarchingCubesCore.triTable[rowIndex + i] != -1; i += 3) {
            int triIndex = rowIndex + i;

            int index = IndexUtilities.Index3D(pos, voxelData.xz, voxelData.y);
            voxelData.TryGetVoxelDataID(index, out Vector2Int ID);

            VertexData a = new VertexData(vertexList.GetVertex(MarchingCubesCore.triTable[triIndex]), ID);
            VertexData b = new VertexData(vertexList.GetVertex(MarchingCubesCore.triTable[triIndex + 1]), ID);
            VertexData c = new VertexData(vertexList.GetVertex(MarchingCubesCore.triTable[triIndex + 2]), ID);

            vertexData.Add(a);
            vertexData.Add(b);
            vertexData.Add(c);
        }
    }
}