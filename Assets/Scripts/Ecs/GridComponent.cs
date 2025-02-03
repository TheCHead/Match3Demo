using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using UnityEngine;

public struct GridComponent
{
    public int width;
    public int height;
    public float cellSize;
    public Vector3 origin;
    public Entity[,] gridArray;
    public int[,] mask;
    public Grid2D<Gem>.CoordinateConverter coordinateConverter;

    public List<Vector2Int> tileSelection;

    public Entity GetTileValue(int x, int y)
    {
        return gridArray[x, y];
    }

    public void SetTileValue(int x, int y, Entity entity)
    {
        if (IsValidTile(x, y))
        {
            gridArray[x, y] = entity;
        }
    }
    public Entity GetTileValue(Vector2Int gridPos)
    {
        return gridArray[gridPos.x, gridPos.y];
    }
    public bool IsValidTile(int x, int y) => x >= 0 && y >=0 && x < width && y < height && mask[x, y] == 1;
    public bool IsEmptyTile(int x, int y) => IsValidTile(x, y) && (gridArray[x, y] == null || gridArray[x, y] == Entity.Null);
    public bool IsObjectTile(int x, int y) => IsValidTile(x, y) && gridArray[x, y] != null && gridArray[x, y] != Entity.Null;
    public bool IsGemTile(int x, int y) => IsObjectTile(x, y) && gridArray[x, y].Has<GemComponent>();
    public bool IsGemTile(Vector2Int gridPos) => IsGemTile(gridPos.x, gridPos.y);
}
