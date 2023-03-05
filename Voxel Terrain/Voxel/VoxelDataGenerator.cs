using UnityEngine;
using System.Linq;

public static class VoxelDataGenerator {
    public static VoxelData GenerateVoxelDataHeightMap(Color[] pixels, int width, World world, Vector3Int chunkPos) {
        VoxelData voxelData = new VoxelData(World.sizeXZ + 1, World.sizeY + 1, chunkPos);

        for (int x = 0; x < voxelData.xz; x ++) {
            for (int y = 0; y < voxelData.y; y ++) {
                for (int z = 0; z < voxelData.xz; z ++) {
                    Vector2Int samplePos = new Vector2Int(x + chunkPos.x, width - (z + chunkPos.z));
                    Vector3Int pos = new Vector3Int(x, y, z);

                    int pixelIndex = IndexUtilities.Index2D(samplePos, width);
                    int index = IndexUtilities.Index3D(pos, voxelData.xz, voxelData.y);

                    float value;

                    if (y == 0 || y == voxelData.y - 1 || pixelIndex == -1) {
                        value = (y == 0) ? 0f : 1f;
                    }
                    else {
                        float heightMapValue = pixels[pixelIndex].grayscale;

                        value = ((float) y / (heightMapValue * world.heightMapHeight));
                        value = Mathf.Clamp01(value);
                    }


                    voxelData.SetVoxelDataDirect(index, value);
                    voxelData.SetVoxelDataIDDirect(index, Vector2Int.zero);
                }
            }
        }

        return voxelData;
    }

    public static VoxelData GenerateVoxelDataProcedural(World world, Vector3Int chunkPos) {
        VoxelData voxelData = new VoxelData(World.sizeXZ + 1, World.sizeY + 1, chunkPos);

        float[] terrainNoise = NoiseCore.NoiseMap(chunkPos, world.seed, world.terrainNoise, voxelData.xz, voxelData.y, voxelData.xz);
        float[] terrainNoise2 = NoiseCore.NoiseMap(chunkPos, world.seed, world.terrainNoise2, voxelData.xz, voxelData.y, voxelData.xz);
        float[] overlayNoise = NoiseCore.NoiseMap(chunkPos, world.seed, world.overlayNoise, voxelData.xz, voxelData.y, voxelData.xz);
        float[] temperatureNoise = NoiseCore.NoiseMap(chunkPos, world.seed + 1, world.biomes.temperature, voxelData.xz, voxelData.y, voxelData.xz);
        float[] humidityNoise = NoiseCore.NoiseMap(chunkPos, world.seed - 1, world.biomes.humidity, voxelData.xz, voxelData.y, voxelData.xz);

        for (int x = 0; x < voxelData.xz; x ++) {
            for (int y = 0; y < voxelData.y; y ++) {
                for (int z = 0; z < voxelData.xz; z ++) {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    int index3D = IndexUtilities.Index3D(pos, voxelData.xz, voxelData.y);

                    float temperature = temperatureNoise[NoiseCore.SampleNoiseMap(pos, temperatureNoise.Last(), voxelData.xz, voxelData.y, voxelData.xz)];
                    float humidity = humidityNoise[NoiseCore.SampleNoiseMap(pos, humidityNoise.Last(), voxelData.xz, voxelData.y, voxelData.xz)];

                    float value;

                    if (y == 0 || y == voxelData.y - 1) {
                        value = (y == 0) ? 0f : 1f;
                    }
                    else {
                        float sample = terrainNoise[NoiseCore.SampleNoiseMap(pos, terrainNoise.Last(), voxelData.xz, voxelData.y, voxelData.xz)];
                        float sample2 = terrainNoise2[NoiseCore.SampleNoiseMap(pos, terrainNoise2.Last(), voxelData.xz, voxelData.y, voxelData.xz)];
                        float overlaySample = overlayNoise[NoiseCore.SampleNoiseMap(pos, overlayNoise.Last(), voxelData.xz, voxelData.y, voxelData.xz)];

                        value = ((float) y / (sample * sample2)) * overlaySample;
                        value = Mathf.Clamp01(value);
                    }

                    Vector2Int ID = CalculateID(world.biomes, temperature, humidity, (float) y / (voxelData.y - 1));

                    voxelData.SetVoxelDataDirect(index3D, value);
                    voxelData.SetVoxelDataIDDirect(index3D, ID);
                }
            }
        }

        return voxelData;
    }

    static Vector2Int CalculateID(Biomes biomes, float temperature, float humidity, float normalisedY) {
        Vector2Int ID = Vector2Int.zero;
        Biome biome = biomes.CalculateBiome(temperature, humidity, out int biomeIndex);

        ID.y = biomeIndex;

        foreach (BiomeTerrainType biomeTerrainType in biome.biomeTerrainTypes) {
            if (normalisedY <= biomeTerrainType.y) {
                ID.x = biomeTerrainType.terrainTypeID;
                break;
            }
        }

        return ID;
    }
}