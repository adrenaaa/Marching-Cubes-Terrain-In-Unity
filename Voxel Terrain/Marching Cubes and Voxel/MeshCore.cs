using UnityEngine;

public static class MeshCore {
    public static readonly Vector3Int[] cardinalDirections = new Vector3Int[8] {
        Vector3Int.forward, // north
        Vector3Int.right, // east
        Vector3Int.down, // south
        Vector3Int.left, // west
        Vector3Int.forward + Vector3Int.right, // north east
        Vector3Int.right + Vector3Int.down, // south east
        Vector3Int.down + Vector3Int.left, // south west
        Vector3Int.left + Vector3Int.forward // north west
    };

    public static readonly Vector3Int[] corners = new Vector3Int[8] {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 1, 1),
        new Vector3Int(1, 1, 1)
    };
}