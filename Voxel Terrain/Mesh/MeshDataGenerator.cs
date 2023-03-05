using UnityEngine;

public static class MeshDataGenerator {
    public static MeshData GenerateMeshData(VoxelData voxelData, World world) {
        MeshData meshData = new MeshData(voxelData, world);

        return meshData;
    }
}