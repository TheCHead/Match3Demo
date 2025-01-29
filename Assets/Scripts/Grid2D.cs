using System;
using TMPro;
using UnityEngine;

public class Grid2D<T>
{
    readonly int width;
    readonly int height;
    readonly float cellSize;
    readonly Vector3 origin;
    readonly T[,] gridArray;
    readonly CoordinateConverter coordinateConverter;
    public event Action<int, int, T> OnGridValueChanged;

    /// <summary>
    /// Factory method for Vectical 2D Grid.
    /// </summary>
    public static Grid2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new Grid2D<T>(width, height, cellSize, origin, new VerticalConverter(), debug);
    }

    /// <summary>
    /// Factory method for Horizontal 2D Grid.
    /// </summary>
    public static Grid2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new Grid2D<T>(width, height, cellSize, origin, new HorizontalConverter(), debug);
    }

    public Grid2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter, bool debug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        this.coordinateConverter = coordinateConverter ?? new VerticalConverter();
        gridArray = new T[width, height];

        if (debug)
        {
            DrawDebugLines();
        }
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
    public bool IsValid(int x, int y) => x >= 0 && y >=0 && x < width && y < height;
    public bool IsEmpty(int x, int y) => GetValue(x, y) == null;

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

    public abstract class CoordinateConverter
    {
        public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
        public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);
        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);
        public abstract Vector3 Forward { get; }
    }

    /// <summary>
    /// Converter for 2D grid on XY plane
    /// </summary>
    public class VerticalConverter : CoordinateConverter
    {
        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x, y, 0) * cellSize + origin;
        }

        public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
        {
            Vector3 gridPosition = (worldPosition - origin)/cellSize;
            int x = Mathf.FloorToInt(gridPosition.x);
            int y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }

        public override Vector3 Forward => Vector3.forward;
    }

    /// <summary>
    /// Converter for 2D grid on XZ plane
    /// </summary>
    public class HorizontalConverter : CoordinateConverter
    {
        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x, 0, y) * cellSize + origin;
        }

        public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
        {
            Vector3 gridPosition = (worldPosition - origin)/cellSize;
            int x = Mathf.FloorToInt(gridPosition.x);
            int y = Mathf.FloorToInt(gridPosition.z);
            return new Vector2Int(x, y);
        }

        public override Vector3 Forward => Vector3.down;
    }
}
