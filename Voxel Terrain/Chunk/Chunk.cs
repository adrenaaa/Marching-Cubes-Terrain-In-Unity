using UnityEngine;

public class Chunk {
    World world;

    public VoxelData voxelData;
    MeshData meshData;

    Vector3Int chunkPos;

    GameObject chunkObj;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public bool waitingForMesh;
    public bool readyToUpdate;

    public Chunk(World world, Vector3Int chunkPos) {
        this.world = world;
        this.chunkPos = chunkPos;

        GenerateChunkObject();

        GenerateChunkData();
    }

    void GenerateChunkObject() {
        chunkObj = new GameObject($"Chunk {chunkPos}");

        meshFilter = chunkObj.AddComponent<MeshFilter>();
        meshRenderer = chunkObj.AddComponent<MeshRenderer>();
        meshCollider = chunkObj.AddComponent<MeshCollider>();

        chunkObj.transform.position = chunkPos;
        chunkObj.transform.SetParent(world.transform);

        chunkObj.layer = 3;

        meshRenderer.material = world.terrainMaterial;

        if (world.useWater) {
            GenerateWater();
        }
    }

    void GenerateWater() {
        Vector3 waterPos = chunkPos + new Vector3(World.sizeXZ * 0.5f, world.waterLevel, World.sizeXZ * 0.5f);

        GameObject water = MonoBehaviour.Instantiate(world.waterPrefab, waterPos, Quaternion.identity);

        water.transform.localScale = Vector3.one * 0.1f * World.sizeXZ;
        water.transform.SetParent(chunkObj.transform);
    }

    void GenerateChunkData() {
        world.chunkDataGenerator.RequestVoxelData(OnVoxelDataReceived, chunkPos);
    }

    void OnVoxelDataReceived(VoxelData voxelData) {
        this.voxelData = voxelData;

        RequestMeshData(true);
    }

    void RequestMeshData(bool autoApply) {
        waitingForMesh = true;

        world.chunkDataGenerator.RequestMeshData(autoApply ? OnMeshDataReceivedAutoApply : OnMeshDataReceived, voxelData);
    }

    void OnMeshDataReceivedAutoApply(MeshData meshData) {
        OnMeshDataReceived(meshData);

        ApplyMesh();
    }

    void OnMeshDataReceived(MeshData meshData) {
        this.meshData = meshData;
        meshData.Generate();

        waitingForMesh = false;
    }

    public void SetChunkActive(bool active) {
        chunkObj.SetActive(active);
    }

    public void RegenerateMesh(bool autoApply) {
        voxelData.GetVoxelDataFromStorage(world.voxelDataStorage);
        voxelData.GetVoxelDataIDsFromStorage(world.voxelDataStorage);

        RequestMeshData(autoApply);

        readyToUpdate = false;
    }

    public void ApplyMesh() {
        meshFilter.mesh = meshData.mesh;
        meshCollider.sharedMesh = meshData.mesh;
    }
}