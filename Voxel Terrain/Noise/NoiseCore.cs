using UnityEngine;
using Unity.Mathematics;

// this script works with fastnoise so i can have noisemaps with variety and dont need to use unitys dumb perlin function
// which is literally rng no pun intended
public static class NoiseCore {
    // creates a new fastnoise
    public static FastNoise CreateFastNoise(NoiseSettings noiseSettings, int seed) {
        FastNoise fastNoise = new FastNoise(seed);

        fastNoise.SetNoiseType(noiseSettings.noiseType);
        fastNoise.SetRotationType3D(noiseSettings.rotationType3D);
        fastNoise.SetFrequency(noiseSettings.frequency);

        fastNoise.SetFractalType(noiseSettings.fractalType);
        fastNoise.SetFractalOctaves(noiseSettings.fractalOctaves);
        fastNoise.SetFractalLacunarity(noiseSettings.fractalLacunarity);
        fastNoise.SetFractalGain(noiseSettings.fractalGain);
        fastNoise.SetFractalWeightedStrength(noiseSettings.fractalWeightedStrength);
        fastNoise.SetFractalPingPongStrength(noiseSettings.fractalPingPongStrength);

        fastNoise.SetCellularDistanceFunction(noiseSettings.cellularDistanceFunction);
        fastNoise.SetCellularReturnType(noiseSettings.cellularReturnType);
        fastNoise.SetCellularJitter(noiseSettings.cellularJitter);

        fastNoise.SetDomainWarpType(noiseSettings.domainWarpType);
        fastNoise.SetDomainWarpAmp(noiseSettings.domainAmplitude);

        return fastNoise;
    }

    public static int SampleNoiseMap(Vector3Int pos, float mapType, int sizeX, int sizeY, int sizeZ) {
        if (mapType == 0f) {
            return IndexUtilities.Index2D(pos.x, pos.z, sizeX);
        }

        return IndexUtilities.Index3D(pos.z, pos.y, pos.x, sizeX, sizeY);
    }

    // creates a noise map
    public static float[] NoiseMap(Vector3 offset, int seed, NoiseSettings noiseSettings, int sizeX, int sizeY, int sizeZ) {
        float[] noiseMap;

        // initialises the fastnoise and noise map
        FastNoise fastNoise = CreateFastNoise(noiseSettings, seed);

        // increments every iteration
        int index = 0;

        // if not 3d make 2d else make 3d
        if (!noiseSettings.threeDimensional) {
            noiseMap = new float[(sizeX * sizeZ) + 1];

            for (int x = 0; x < sizeX; x ++) {
                for (int y = 0; y < sizeZ; y ++) { 
                    // samples the fastnoise using the offset given
                    float sampleX = offset.x + x;
                    float sampleY = offset.z + y;
                    
                    // sample the noise
                    float sample = fastNoise.GetNoise(sampleX, sampleY);
                    
                    // does some math to convert the noise from a -1 to 1 to a 0 to 1 ratio if needs be
                    if (noiseSettings.convertTo01Ratio) {
                        sample = (sample + 1f) * 0.5f;
                    }

                    // assigns the noise sample to the noise map
                    noiseMap[index ++] = sample * noiseSettings.multiplier;
                }
            }

            // determines wether its 2d or 3d so we can index it
            noiseMap[noiseMap.Length - 1] = 0f;
        }
        // literally the exact same as the other one but 3d
        else {
            noiseMap = new float[(sizeX * sizeY * sizeZ) + 1];

            for (int x = 0; x < sizeX; x ++) {
                for (int y = 0; y < sizeY; y ++) {
                    for (int z = 0; z < sizeZ; z ++) {
                        float sampleX = offset.x + x;
                        float sampleY = offset.y + y;
                        float sampleZ = offset.z + z;

                        float sample = fastNoise.GetNoise(sampleX, sampleY, sampleZ);

                        if (noiseSettings.convertTo01Ratio) {
                            sample = (sample + 1f) * 0.5f;
                        }

                        // assigns the noise sample to the noise map
                        noiseMap[index ++] = sample * noiseSettings.multiplier;
                    }
                }
            }

            // determines wether its 2d or 3d so we can index it
            noiseMap[noiseMap.Length - 1] = 1f;

        }

        return noiseMap;
    }
}

// all the noise settings that i definitiely did not steal from the fastnoise lite gui
[System.Serializable]
public class NoiseSettings {
    [Header("General")]
    public FastNoise.NoiseType noiseType = FastNoise.NoiseType.OpenSimplex2;
    public FastNoise.RotationType3D rotationType3D = FastNoise.RotationType3D.None;
    public float frequency = 0.01f;
    
    [Header("Miscellaneous")]
    public float multiplier = 1f;
    public bool convertTo01Ratio = true;
    public bool threeDimensional;

    [Header("Fractal")]
    public FastNoise.FractalType fractalType = FastNoise.FractalType.FBm;
    public int fractalOctaves = 5;
    public float fractalLacunarity = 2f;
    public float fractalGain = 0.5f;
    public float fractalWeightedStrength = 0f;
    public float fractalPingPongStrength = 2f;

    [Header("Cellular")]
    public FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.EuclideanSq;
    public FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.Distance;
    public float cellularJitter = 2f;

    [Header("Domain Warp")]
    public FastNoise.DomainWarpType domainWarpType = FastNoise.DomainWarpType.OpenSimplex2;
    public float domainAmplitude = 30f;
}
