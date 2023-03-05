using UnityEngine;

public class TerrainTypeContainer : MonoBehaviour {
    public World world;
    [Space(5f)]
    public TerrainType[] terrainTypes;

    const int texSize = 1024;

    Texture2DArray texturesTop;
    Texture2DArray texturesSides;
    Texture2DArray texturesTopNormals;
    Texture2DArray texturesSidesNormals;

    void Awake() {
        InitialiseTextureArrays();
    }

    void InitialiseTextureArrays() {
        texturesTop = new Texture2DArray(texSize, texSize, terrainTypes.Length, TextureFormat.RGBA32, true);
        texturesSides = new Texture2DArray(texSize, texSize, terrainTypes.Length, TextureFormat.RGBA32, true);
        texturesTopNormals = new Texture2DArray(texSize, texSize, terrainTypes.Length, TextureFormat.RGBA32, true, true);
        texturesSidesNormals = new Texture2DArray(texSize, texSize, terrainTypes.Length, TextureFormat.RGBA32, true, true);

        int index = 0;

        foreach (TerrainType terrainType in terrainTypes) {
            texturesTop.SetPixels(terrainType.top.GetPixels(), index);
            texturesSides.SetPixels(terrainType.sides.GetPixels(), index);
            texturesTopNormals.SetPixels(terrainType.topNormal.GetPixels(), index);
            texturesSidesNormals.SetPixels(terrainType.sidesNormals.GetPixels(), index);

            index ++;
        }

        texturesTop.Apply();
        texturesSides.Apply();
        texturesTopNormals.Apply();
        texturesSidesNormals.Apply();

        world.terrainMaterial.SetTexture("_Textures_Top", texturesTop);
        world.terrainMaterial.SetTexture("_Textures_Sides", texturesSides);
        world.terrainMaterial.SetTexture("_Textures_Top_Normals", texturesTopNormals);
        world.terrainMaterial.SetTexture("_Textures_Sides_Normals", texturesSidesNormals);
    }
}

[System.Serializable]
public struct TerrainType {
    public string type;
    [Space(5f)]
    public Texture2D top;
    public Texture2D sides;
    [Space(5f)]
    public Texture2D topNormal;
    public Texture2D sidesNormals;
}