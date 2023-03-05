using UnityEngine;

public static class IndexUtilities {
    public static int Index2D(Vector2Int pos, int width) {
        return Index2D(pos.x, pos.y, width);
    }

    public static int Index2D(int x, int y, int width) {
        if (x >= width || x < 0) return -1;
        if (y >= width || y < 0) return -1;

        return x * width + y;
    }
    
    public static int Index3D(Vector3Int pos, int width, int height) {
        return Index3D(pos.x, pos.y, pos.z, width, height);
    }

    public static int Index3D(int x, int y, int z, int width, int height) {
        if (x >= width || x < 0) return -1;
        if (y >= height || y < 0) return -1;
        if (z >= width || z < 0) return -1;

        return z * width * height + y * width + x;
    }
}