using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class MeshData {
    public Mesh mesh;

    VoxelData voxelData;
    World world;

    public MeshData(VoxelData voxelData, World world) {
        this.voxelData = voxelData;
        this.world = world;
    }

    public void Generate() {
        NativeList<VertexData> vertexData = new NativeList<VertexData>(Allocator.TempJob);

        switch (world.meshType) {
            case MeshType.MarchingCubesInterpolated or MeshType.MarchingCubesMidpoints:

                MarchingCubesJob marchingCubesJob = new MarchingCubesJob (
                    vertexData,
                    voxelData,
                    world.isoLevel,
                    world.meshType == MeshType.MarchingCubesMidpoints
                );

                JobHandle marchingCubesJobHandle = marchingCubesJob.Schedule();
                marchingCubesJobHandle.Complete();

            break;

            case MeshType.Voxel:

                VoxelJob voxelJob = new VoxelJob (
                    vertexData, 
                    voxelData, 
                    world.isoLevel
                );

                JobHandle voxelJobHandle = voxelJob.Schedule();
                voxelJobHandle.Complete();

            break;
        }

        voxelData.StoreVoxelData(world.voxelDataStorage);
        voxelData.StoreVoxelDataIDs(world.voxelDataStorage);

        MeshPackage meshPackage = UnpackVertexData(vertexData);
        vertexData.Dispose();

        ConstructMesh(meshPackage);
    }

    MeshPackage UnpackVertexData(NativeList<VertexData> vertexData) {
        NativeList<Vector3> vertices = new NativeList<Vector3>(Allocator.TempJob);
        NativeList<int> triangles = new NativeList<int>(Allocator.TempJob);
        NativeList<Vector2> uvs = new NativeList<Vector2>(Allocator.TempJob);

        VoxelDataUnpacker voxelDataUnpacker = new VoxelDataUnpacker (
            vertexData,
            vertices,
            triangles,
            uvs,
            world.smoothShading
        );

        JobHandle jobHandle = voxelDataUnpacker.Schedule();
        jobHandle.Complete();

        MeshPackage meshPackage = new MeshPackage(vertices.ToArray(), triangles.ToArray(), uvs.ToArray());

        vertices.Dispose();
        triangles.Dispose();
        uvs.Dispose();

        return meshPackage;
    }

    void ConstructMesh(MeshPackage meshPackage) {
        mesh = new Mesh();

        mesh.vertices = meshPackage.vertices;
        mesh.triangles = meshPackage.triangles;
        mesh.uv = meshPackage.uvs;

        mesh.RecalculateNormals();
    }

    struct MeshPackage {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        public MeshPackage(Vector3[] vertices, int[] triangles, Vector2[] uvs) {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}