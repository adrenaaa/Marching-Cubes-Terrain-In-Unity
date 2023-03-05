public struct IndiceList {
    int indice1;
    int indice2;
    int indice3;
    int indice4;
    int indice5;
    int indice6;

    public int GetIndice(int index) {
        switch (index) {
            case 0: return indice1;
            case 1: return indice2;
            case 2: return indice3;
            case 3: return indice4;
            case 4: return indice5;
            case 5: return indice6;

            default: throw new System.IndexOutOfRangeException($"Index {index} does not exist in indice list. (Get)");
        }
    }

    public void SetIndice(int index, int indice) {
        switch (index) {
            case 0:
                indice1 = indice;
            break;

            case 1:
                indice2 = indice;
            break;

            case 2:
                indice3 = indice;
            break;

            case 3:
                indice4 = indice;
            break;

            case 4:
                indice5 = indice;
            break;

            case 5:
                indice6 = indice;
            break;

            default: throw new System.IndexOutOfRangeException($"Index {index} does not exist in indice list. (Set)");
        }
    }
}