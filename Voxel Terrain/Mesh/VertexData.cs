using UnityEngine;

public struct VertexData {
    public Vector3 pos;
    public Vector2Int ID;

    public VertexData(Vector3 pos, Vector2Int ID){
        this.pos = pos;
        this.ID = ID;
    }
}