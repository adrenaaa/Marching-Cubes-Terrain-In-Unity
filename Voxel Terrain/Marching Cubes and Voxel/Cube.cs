public struct Cube {
    public float corner0;
    public float corner1;
    public float corner2;
    public float corner3;
    public float corner4;
    public float corner5;
    public float corner6;
    public float corner7;

    public void SetCorner(int corner, float value) {
        switch (corner) {
            case 0:
                corner0 = value;
            break;

            case 1:
                corner1 = value;
            break;

            case 2:
                corner2 = value;
            break;

            case 3:
                corner3 = value;
            break;

            case 4:
                corner4 = value;
            break;

            case 5:
                corner5 = value;
            break;

            case 6:
                corner6 = value;
            break;

            case 7:
                corner7 = value;
            break;
        }
    }

    public float GetCorner(int corner) {
        switch (corner) {
            case 0: return corner0;
            case 1: return corner1;
            case 2: return corner2;
            case 3: return corner3;
            case 4: return corner4;
            case 5: return corner5;
            case 6: return corner6;
            case 7: return corner7;

            default: return -1f;
        }
    }
}