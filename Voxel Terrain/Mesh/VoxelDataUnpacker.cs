using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct VoxelDataUnpacker : IJob {
    [NativeDisableParallelForRestriction] public NativeList<VertexData> vertexData;

    [NativeDisableParallelForRestriction] public NativeList<Vector3> vertices;
    [NativeDisableParallelForRestriction] public NativeList<int> triangles;
    [NativeDisableParallelForRestriction] public NativeList<Vector2> uvs;

    public bool smoothShading;

    public VoxelDataUnpacker(NativeList<VertexData> vertexData, NativeList<Vector3> vertices, NativeList<int> triangles, NativeList<Vector2> uvs, bool smoothShading) {
        this.vertexData = vertexData;
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
        this.smoothShading = smoothShading;
    }

    void IJob.Execute() {
        NativeHashMap<Vector3, int> vertexDataTracker = new NativeHashMap<Vector3, int>(vertexData.Length, Allocator.Temp);

        int triIndex = 0;

        foreach (VertexData vertexData in this.vertexData) {
            Vector3 indexPos = vertexData.pos + (new Vector3(vertexData.ID.x, vertexData.ID.y, 0f) * 99999f);

            if (vertexDataTracker.TryGetValue(indexPos, out int sharedVertexIndex) && smoothShading) {
                triangles.Add(sharedVertexIndex);
            }
            else {
                if (smoothShading) {
                    vertexDataTracker.Add(indexPos, triIndex);
                }

                vertices.Add(vertexData.pos);
                triangles.Add(triIndex);
                uvs.Add(vertexData.ID);

                triIndex ++;
            }
        }

        vertexDataTracker.Dispose();
    }
}