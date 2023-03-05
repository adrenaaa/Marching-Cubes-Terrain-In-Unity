using UnityEngine;

public class Biomes : MonoBehaviour {
    public NoiseSettings temperature;
    public NoiseSettings humidity;
    [Space(5f)]
    public Biome[] biomes;

    public Biome CalculateBiome(float temperature, float humidity, out int biomeIndex) {
        Vector2 tempHumid = new Vector2(temperature, humidity);
        float minDst = float.MaxValue;

        biomeIndex = 0;
        int index = 0;

        foreach (Biome biome in biomes) {
            float dst = (tempHumid - biome.temperatureHumidity.tempHumid).magnitude;

            if (dst < minDst) {
                biomeIndex = index;
                minDst = dst;
            }

            index ++;
        }

        return biomes[biomeIndex];
    }
}

[System.Serializable]
public struct Biome {
    public string biomeName;
    [Space(5f)]
    public TemperatureHumidity temperatureHumidity;
    [Space(5f)]
    public BiomeTerrainType[] biomeTerrainTypes;
}

[System.Serializable]
public struct TemperatureHumidity {
    [Range(-1f, 1f)] public float temperature;
    [Range(-1f, 1f)] public float humidity;

    public Vector2 tempHumid {
        get {
            return new Vector2(temperature, humidity);
        }
    }
}

[System.Serializable]
public struct BiomeTerrainType {
    [Range(0f, 1f)] public float y;
    public int terrainTypeID;
}