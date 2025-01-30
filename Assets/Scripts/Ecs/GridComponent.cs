using UnityEngine;

public struct GridComponent
{
    public int width;
    public int height;
    public float cellSize;
    public Vector3 origin;
    public Gem[,] gridArray;
    public int[,] mask;
    public Grid2D<Gem>.CoordinateConverter coordinateConverter;

    public bool IsValidTile(int x, int y) => x >= 0 && y >=0 && x < width && y < height && mask[x, y] == 1;
}
