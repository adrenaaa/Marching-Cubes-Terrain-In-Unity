using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VoxelTerraformer : MonoBehaviour {
    [Header("World")]
    public World world;
    
    [Header("Terraformation Settings")]
    public float weight;
    public int range;
    [Space(5f)]
    public float maxDst = 250f;
    public LayerMask terrainLayer;

    [Header("Brush")]
    public Transform brush;
    public Material brushMat;
    [Space(5f)]
    public float scaleSmooth = 5f;
    [Space(5f)]
    public float radiusChangeSens = 0.5f;
    public Vector2Int rangeClamp = new Vector2Int(1, 15);
    [Space(5f)]
    public float weightChangeSens = 0.5f;
    public Vector2 weightClamp = new Vector2(1f, 30f);
    [Space(5f)]
    public Color idleIn;
    public Color idleOut;
    public Color removeIn;
    public Color removeOut;
    public Color addIn;
    public Color addOut;
    [Space(5f)]
    public Vector2 noiseScaleMinMax = new Vector2(0.05f, 1f);
    public Vector2 noiseDirMinMax = new Vector2(0.05f, 1f);
    [Space(5f)]
    public float freqScaleSmooth = 5f;

    float rangeAdd;
    float targetNoiseScale;
    float noiseScaleVel;

    int oldRange;

    [HideInInspector] public bool modifyingRange;
    bool terraforming;

    Vector3 brushScaleVel;

    Vector2 targetNoiseDir;
    Vector2 noiseDirVel;

    void Start() {
        ModifyBrushNoise();
    }

    void Update() {
        ModifyRadiusAndWeight();
        DetectModification();
        ModifyBrushNoiseVisual();
    }
    
    void DetectModification() {
        bool canModify = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1);
        bool isHitting = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDst, terrainLayer);

        brush.gameObject.SetActive(isHitting);

        if (isHitting) {
            brush.position = hit.point;

            if (canModify) {
                bool remove = Input.GetKey(KeyCode.Mouse1);

                if (!terraforming) {
                    ModifyTerrain(remove, hit.point);
                }

                ModifyBrushColour(remove, false);
            }
            else {
                ModifyBrushColour(false, true);
            }
        }
    }

    void ModifyBrushColour(bool remove, bool idle) {
        if (idle) {
            brushMat.SetColor("_Colour_In", idleIn);
            brushMat.SetColor("_Colour_Out", idleOut);

            return;
        }

        if (remove) {
            brushMat.SetColor("_Colour_In", removeIn);
            brushMat.SetColor("_Colour_Out", removeOut);
        }
        else {
            brushMat.SetColor("_Colour_In", addIn);
            brushMat.SetColor("_Colour_Out", addOut);
        }
    }

    void ModifyBrushNoiseVisual() {
        float t = freqScaleSmooth * Time.deltaTime;

        Vector2 noiseDir = Vector2.SmoothDamp(brushMat.GetVector("_Noise_Dir"), targetNoiseDir, ref noiseDirVel, t);
        float noiseScale = Mathf.SmoothDamp(brushMat.GetFloat("_Noise_Scale"), targetNoiseScale, ref noiseScaleVel, t);

        brushMat.SetVector("_Noise_Dir", noiseDir);
        brushMat.SetFloat("_Noise_Scale", noiseScale);
    }

    void ModifyBrushNoise() {
        float weight01 = Mathf.InverseLerp(weightClamp.x, weightClamp.y, weight);

        targetNoiseScale = Remap(weight01, 0f, 1f, noiseScaleMinMax.x, noiseScaleMinMax.y);
        targetNoiseDir = Vector2.one * Remap(weight01, 0f, 1f, noiseDirMinMax.x, noiseDirMinMax.y);
    }

    void ModifyRadiusAndWeight() {
        brush.localScale = Vector3.SmoothDamp(brush.localScale, Vector3.one * range, ref brushScaleVel, scaleSmooth * Time.deltaTime);
        modifyingRange = Input.GetKey(KeyCode.Tab);

        int scroll = Mathf.FloorToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 11f);

        if (scroll != 0) {
            weight = Mathf.Clamp(weight + scroll * weightChangeSens, weightClamp.x, weightClamp.y);

            ModifyBrushNoise();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            rangeAdd = 0f;
            oldRange = range;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKey(KeyCode.Tab)) {
            rangeAdd += Input.GetAxisRaw("Mouse Y") * radiusChangeSens;
            int floorRangeAdd = Mathf.FloorToInt(rangeAdd);

            range = Mathf.Clamp(oldRange - floorRangeAdd, rangeClamp.x, rangeClamp.y);
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // * https://stackoverflow.com/questions/3451553/value-remapping
    float Remap(float val, float start1, float stop1, float start2, float stop2) {
        float remappedValue = start2 + (stop2 - start2) * ((val - start1) / (stop1 - start1));
        remappedValue = Mathf.Clamp(remappedValue, start2, stop2);

        return remappedValue;
    }

    async void ModifyTerrain(bool remove, Vector3 point) {
        if (!remove && (point - transform.position).magnitude <= range) {
            return;
        }

        terraforming = true;

        Vector3Int roundedPoint = new Vector3Int (
            Mathf.CeilToInt(point.x),
            Mathf.CeilToInt(point.y),
            Mathf.CeilToInt(point.z)
        );

        float weight = this.weight;

        if (!remove) {
            weight *= -1f;
        }

        List<Chunk> chunksToUpdate = new List<Chunk>();

        for (int x = -range; x <= range; x ++) {
            for (int y = -range; y <= range; y ++) {
                for (int z = -range; z <= range; z ++) {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    Vector3Int samplePoint = roundedPoint + offset;

                    if (samplePoint.y <= 0 || samplePoint.y >= World.sizeY) {
                        continue;
                    }

                    float dst = (roundedPoint - samplePoint).magnitude;

                    if (dst >= range) {
                        continue;
                    }

                    float mult = Mathf.Lerp(1f, 0.5f, dst / range);

                    Vector3Int[] chunkPoss = world.chunkStorage.GetChunkPossAroundPoint(samplePoint);

                    foreach (Vector3Int chunkPos in chunkPoss) {
                        if (world.chunkStorage.TryGetChunk(chunkPos, out Chunk chunk)) {
                            int index = IndexUtilities.Index3D(samplePoint - chunkPos, chunk.voxelData.xz, chunk.voxelData.y);

                            float newWeight = weight * mult * Time.deltaTime;

                            if (world.voxelDataStorage.TryModifyVoxelDataDirect(chunkPos, index, newWeight, true)) {
                                if (!chunk.readyToUpdate) {
                                    chunksToUpdate.Add(chunk);
                                    chunk.readyToUpdate = true;
                                }
                            }
                        }
                    }
                }
            }   
        }

        foreach (Chunk chunk in chunksToUpdate) {
            chunk.RegenerateMesh(false);

            while (chunk.waitingForMesh) {
                await Task.Delay(1);
            }
        }

        foreach (Chunk chunk in chunksToUpdate) {
            chunk.ApplyMesh();
        }

        terraforming = false;
    }
}