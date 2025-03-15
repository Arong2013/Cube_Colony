public class PiceGridHandler
{
    readonly int size;
    int[,,] piceData;
    public PiceGridHandler(Cube cube)
    {
        this.size = cube.Size;

        piceData = new int[size,size,size];

        int id = 0;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                    piceData[x, y, z] = id++;
    }


    public void RotateFace(int layer, string axis)
    {
        int[,] face = new int[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                face[i, j] = (axis == "x") ? piceData[layer, i, j] :
                             (axis == "y") ? piceData[i, layer, j] :
                                             piceData[i, j, layer];
        face = RotateMatrix90(face);

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                if (axis == "x") piceData[layer, i, j] = face[i, j];
                else if (axis == "y") piceData[i, layer, j] = face[i, j];
                else piceData[i, j, layer] = face[i, j];
    }
    int[,] RotateMatrix90(int[,] matrix)
    {
        int[,] rotated = new int[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                rotated[j, size-1 - i] = matrix[i, j]; 
        for (int i = 0; i < size; i++)
        {
            string row = "";
            for (int j = 0; j < size; j++)
                row += rotated[i, j] + " ";
        }

        return rotated;
    }
}