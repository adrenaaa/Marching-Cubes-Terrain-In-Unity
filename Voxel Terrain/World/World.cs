using UnityEngine;
using System.Collections.Generic;

public enum MeshType {
    MarchingCubesInterpolated,
    MarchingCubesMidpoints,
    Voxel
}

public enum VoxelDataType {
    Procedural,
    HeightMap
}

public class World : MonoBehaviour {
    [Header("Generators")]
    public ChunkDataGenerator chunkDataGenerator;

    [Header("Storage")]
    public ChunkStorage chunkStorage;
    public VoxelDataStorage voxelDataStorage;

    [Header("Biomes")]
    public Biomes biomes;
    public TerrainTypeContainer terrainTypeContainer;

    [Header("Mesh")]
    public Material terrainMaterial;

    [Header("Water")]
    public bool useWater;
    public GameObject waterPrefab;
    public float waterLevel = 8f;

    [Header("Noise")]
    public int seed;
    public bool randomSeed;
    [Space(5f)]
    public NoiseSettings terrainNoise;
    public NoiseSettings terrainNoise2;
    public NoiseSettings overlayNoise;

    [Header("Generation")]
    public int renderDistance = 7;
    [Space(5f)]
    public bool smoothShading;
    [Space(5f)]
    public MeshType meshType;
    public VoxelDataType voxelDataType;

    [Header("Height Map")]
    public Texture2D heightMap;
    public float heightMapHeight = 50f;

    [Header("Marching Cubes")]
    [Range(0f, 1f)] public float isoLevel = 0.5f;

    [Header("Endless Terrain")]
    public bool endless;
    [Space(5f)]
    public Transform target;

    public const int sizeXZ = 16;
    public const int sizeY = 80;

    int minChunkDst;

    Vector3Int targetPos;
    Vector3Int oldTargetPos;

    Queue<Chunk> updatedChunksInLastUpdate = new Queue<Chunk>();
    List<Chunk> activeChunks = new List<Chunk>();

    bool TryGenerate() {
        return targetPos != oldTargetPos;
    }

    void Awake() {
        minChunkDst = renderDistance * sizeXZ;

        if (randomSeed) {
            RandomiseSeed();
        }
    }

    void Start() {
        if (voxelDataType == VoxelDataType.HeightMap) {
            GenerateChunksHeightMap();
        }
        else {
            GenerateChunksProcedural();
        }
    }

    void Update() {
        if (!endless || voxelDataType == VoxelDataType.HeightMap) {
            return;
        }

        targetPos = new Vector3Int (
            Mathf.RoundToInt(target.position.x / sizeXZ) * sizeXZ,
            0,
            Mathf.RoundToInt(target.position.z / sizeXZ) * sizeXZ
        );
        
        if (TryGenerate()) {
            GenerateChunksProcedural();
        }
    }

    void LateUpdate() {
        oldTargetPos = new Vector3Int (
            Mathf.RoundToInt(target.position.x / sizeXZ) * sizeXZ,
            0,
            Mathf.RoundToInt(target.position.z / sizeXZ) * sizeXZ
        );
    }

    void RandomiseSeed() {
        seed = Random.Range(int.MinValue + 1, int.MaxValue - 1);
    }

    void GenerateChunksProcedural() {
        activeChunks.Clear();

        for (int i = 0; i < updatedChunksInLastUpdate.Count; i ++) {
            Chunk chunk = updatedChunksInLastUpdate.Dequeue();

            chunk.SetChunkActive(false);
        }

        for (int x = -renderDistance; x < renderDistance; x ++) {
            for (int y = -renderDistance; y < renderDistance; y ++) {
                Vector3Int chunkPos = new Vector3Int(x, 0, y) * sizeXZ + targetPos;

                if (!chunkStorage.ContainsChunk(chunkPos)) {
                    GenerateChunk(chunkPos);
                }

                UpdateChunk(chunkPos);
            }
        }
    }

    void GenerateChunksHeightMap() {
        int sizeX = heightMap.width / sizeXZ;
        int sizeY = heightMap.height / sizeXZ;

        for (int x = 0; x < sizeX; x ++) {
            for (int y = 0; y < sizeY; y ++) {
                Vector3Int chunkPos = new Vector3Int(x, 0, y) * sizeXZ;

                GenerateChunk(chunkPos);
            }
        }
    }

    void UpdateChunk(Vector3Int chunkPos) {
        Vector3Int size = new Vector3Int(minChunkDst, 0, minChunkDst);
        Bounds bounds = new Bounds(targetPos, size);

        bool active = bounds.Contains(chunkPos);

        chunkStorage.TrySetChunkActive(chunkPos, active);
        chunkStorage.TryGetChunk(chunkPos, out Chunk chunk);

        if (active) {
            activeChunks.Add(chunk);
        }

        updatedChunksInLastUpdate.Enqueue(chunk);
    }

    void GenerateChunk(Vector3Int chunkPos) {
        Chunk chunk = new Chunk(this, chunkPos);

        chunkStorage.StoreChunk(chunkPos, chunk);
    }
}