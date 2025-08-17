[System.Serializable]
public class MapData
{
    public int rows;
    public int columns;
    public int[] gridData; // flatten từ int[,]
    public int[] itemInstanceId; // flatten từ int[,]

    private int[] Flatten2DArray(int[,] array2D, int rows, int cols)
    {
        int[] flat = new int[rows * cols];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                flat[y * cols + x] = array2D[y, x];
            }
        }

        return flat;
    }

    private int[,] Unflatten2DArray(int[] flat, int rows, int cols)
    {
        int[,] array2D = new int[rows, cols];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                array2D[y, x] = flat[y * cols + x];
            }
        }

        return array2D;
    }
}