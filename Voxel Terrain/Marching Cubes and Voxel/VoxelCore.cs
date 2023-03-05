using UnityEngine;
using System.Collections.Generic;

public static class VoxelCore {
    public static readonly int[] triTable = new int[36] {
        7, 3, 0, 0, 4, 7, // right x
        5, 1, 2, 2, 6, 5, // left -x
        7, 4, 5, 5, 6, 7, // up y
        2, 1, 0, 0, 3, 2, // down -y
        6, 2, 3, 3, 7, 6, // forward z
        4, 0, 1, 1, 5, 4, // back -z
    };

    public static readonly Vector3Int[] neighbourSampleDirections = new Vector3Int[6] {
        Vector3Int.right,
        Vector3Int.left,
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.back,
    };
}