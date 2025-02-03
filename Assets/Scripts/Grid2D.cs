using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class Grid2D<T>
{
    readonly int width;
    readonly int height;
    readonly float cellSize;
    readonly Vector3 origin;
    readonly T[,] gridArray;
    readonly int[,] mask;
    readonly CoordinateConverter coordinateConverter;
    public event Action<int, int, T> OnGridValueChanged;

    /// <summary>
    /// Factory method for Vectical 2D Grid.
    /// </summary>
    public static Grid2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new Grid2D<T>(width, height, cellSize, origin, new VerticalConverter(), GetDefaultMask(width, height), debug);
    }

    /// <summary>
    /// Factory method for Horizontal 2D Grid.
    /// </summary>
    public static Grid2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new Grid2D<T>(width, height, cellSize, origin, new HorizontalConverter(), GetDefaultMask(width, height), debug);
    }

    public static Grid2D<T> TilemapGrid(Tilemap tilemap, bool debug = false)
    {

        return new Grid2D<T>(tilemap.size.x, tilemap.size.y, tilemap.cellSize.x, tilemap.origin, new VerticalConverter(), GetTilemapMask(tilemap), debug);
    }

    public Grid2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter, int[,] gridMask, bool debug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        this.coordinateConverter = coordinateConverter ?? new VerticalConverter();
        gridArray = new T[width, height];
        mask = gridMask;

        if (debug)
        {
            DrawDebugLines();
        }
    }

    private static int[,] GetDefaultMask(int width, int height)
    {
        int[,] gridMask = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridMask[x, y] = 1;
            }
        }

        return gridMask;
    }

    private static int[,] GetTilemapMask(Tilemap tilemap)
    {
        int[,] gridMask = new int[tilemap.size.x, tilemap.size.y];
        for (int x = 0; x < tilemap.size.x; x++)
        {
            for (int y = 0; y < tilemap.size.y; y++)
            {
                gridMask[x, y] = tilemap.HasTile(tilemap.origin + new Vector3Int(x, y, tilemap.origin.z)) ? 1 : 0;
            }
        }

        return gridMask;
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int pos = GetXY(worldPosition);
        SetValue(pos.x, pos.y, value);
    }

    public void SetValue(int x, int y, T value)
    {
        if (IsValid(x, y))
        {
            gridArray[x,y] = value;
            OnGridValueChanged?.Invoke(x, y, value);
        }
    }

    public T GetValue(Vector3 worldPosition)
    {
        Vector2Int pos = GetXY(worldPosition);
        return GetValue(pos.x, pos.y);
    }

    public T GetValue(int x, int y)
    {
        return IsValid(x, y) ? gridArray[x,y] : default(T);
    }

    private Vector3 GetWorldPosition(int x, int y) => coordinateConverter.GridToWorld(x, y, cellSize, origin);
    public Vector3 GetWorldPositionCenter(int x, int y) => coordinateConverter.GridToWorldCenter(x, y, cellSize, origin);
    public Vector2Int GetXY(Vector3 worldPosition) => coordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
    public bool IsValid(int x, int y) => x >= 0 && y >=0 && x < width && y < height && mask[x, y] == 1;
    public bool IsEmpty(int x, int y) => IsValid(x, y) && GetValue(x, y) == null;

    private void DrawDebugLines()
    {
        const float duration = 100f;
        GameObject parent = new GameObject("Debugging");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), coordinateConverter.Forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, duration);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, duration);
    }

    private TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 direction, 
    int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center, int sortingOrder = 0)
    {
        GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = position;
        gameObject.transform.forward = direction;

        TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = color == default ? Color.white : color;
        textMeshPro.alignment = textAnchor;
        textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMeshPro;
    }
}
