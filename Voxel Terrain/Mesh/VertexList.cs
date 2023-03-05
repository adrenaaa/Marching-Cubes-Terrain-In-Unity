using UnityEngine;

public struct VertexList {
    Vector3 vert0;
    Vector3 vert1;
    Vector3 vert2;
    Vector3 vert3;
    Vector3 vert4;
    Vector3 vert5;
    Vector3 vert6;
    Vector3 vert7;
    Vector3 vert8;
    Vector3 vert9;
    Vector3 vert10;
    Vector3 vert11;

    public void SetVertex(int index, Vector3 vert) {
        switch (index) {
            case 0:
                vert0 = vert;
            break;

            case 1:
                vert1 = vert;
            break;

            case 2:
                vert2 = vert;
            break;

            case 3:
                vert3 = vert;
            break;

            case 4:
                vert4 = vert;
            break;

            case 5:
                vert5 = vert;
            break;

            case 6:
                vert6 = vert;
            break;

            case 7:
                vert7 = vert;
            break;

            case 8:
                vert8 = vert;
            break;

            case 9:
                vert9 = vert;
            break;

            case 10:
                vert10 = vert;
            break;

            case 11:
                vert11 = vert;
            break;
        }
    }

    public Vector3 GetVertex(int index) {
        switch (index) {
            case 0: return vert0;
            case 1: return vert1;
            case 2: return vert2;
            case 3: return vert3;
            case 4: return vert4;
            case 5: return vert5;
            case 6: return vert6;
            case 7: return vert7;
            case 8: return vert8;
            case 9: return vert9;
            case 10: return vert10;
            case 11: return vert11;

            default: return -Vector3.one;
        }
    }
}