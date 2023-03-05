using UnityEngine;
using System.Threading;
using System;
using System.Collections.Generic;

public class ChunkDataGenerator : MonoBehaviour {
    public World world;

    Queue<ThreadInfo<VoxelData>> voxelDataQueue = new Queue<ThreadInfo<VoxelData>>();
    Queue<ThreadInfo<MeshData>> meshDataQueue = new Queue<ThreadInfo<MeshData>>();

    Color[] heightMapPixels;
    int width;

    void Start() {
        if (world.voxelDataType == VoxelDataType.HeightMap) {
            heightMapPixels = world.heightMap.GetPixels();
            width = world.heightMap.height;
        }
    }

    void Update() {
        if (voxelDataQueue.Count > 0) {
            for (int i = 0; i < voxelDataQueue.Count; i ++) {
                ThreadInfo<VoxelData> voxelDataThreadInfo = voxelDataQueue.Dequeue();
                voxelDataThreadInfo.callback(voxelDataThreadInfo.param);
            }
        }

        if (meshDataQueue.Count > 0) {
            for (int i = 0; i < meshDataQueue.Count; i ++) {
                ThreadInfo<MeshData> meshDataThreadInfo = meshDataQueue.Dequeue();
                meshDataThreadInfo.callback(meshDataThreadInfo.param);
            }
        }
    }

    #region Voxel Data

    public void RequestVoxelData(Action<VoxelData> callback, Vector3Int chunkPos) {
        ThreadStart start = delegate {
            GenerateVoxelData(callback, chunkPos);
        };

        new Thread(start).Start();
    }

    void GenerateVoxelData(Action<VoxelData> callback, Vector3Int chunkPos) {
        VoxelData voxelData = world.voxelDataType == VoxelDataType.Procedural 
        ? VoxelDataGenerator.GenerateVoxelDataProcedural(world, chunkPos)
        : VoxelDataGenerator.GenerateVoxelDataHeightMap(heightMapPixels, width, world, chunkPos);

        lock (voxelDataQueue) {
            ThreadInfo<VoxelData> voxelDataThreadInfo = new ThreadInfo<VoxelData>(callback, voxelData);
            voxelDataQueue.Enqueue(voxelDataThreadInfo);
        }
    }
    
    #endregion

    #region Mesh Data

    public void RequestMeshData(Action<MeshData> callback, VoxelData voxelData) {
        ThreadStart start = delegate {
            GenerateMeshData(callback, voxelData);
        };

        new Thread(start).Start();
    }

    void GenerateMeshData(Action<MeshData> callback, VoxelData voxelData) {
        MeshData meshData = MeshDataGenerator.GenerateMeshData(voxelData, world);

        lock (meshDataQueue) {
            ThreadInfo<MeshData> meshDataThreadInfo = new ThreadInfo<MeshData>(callback, meshData);
            meshDataQueue.Enqueue(meshDataThreadInfo);
        }
    }
    
    #endregion
    
    struct ThreadInfo<T> {
        public Action<T> callback;
        public T param;

        public ThreadInfo(Action<T> callback, T param) {
            this.callback = callback;
            this.param = param;
        }
    }
}