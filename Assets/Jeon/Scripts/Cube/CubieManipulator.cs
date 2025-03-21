public static class CubieManipulator
{
    public static void RotateCubies(Cubie[,] cubies, bool isClockwise, CubeAxisType axis)
    {
        foreach (var cubie in cubies)
        {
            cubie.RotateCubie(axis, isClockwise);
        }
    }
}