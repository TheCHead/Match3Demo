
public class GridObject<T>
{
    private Grid2D<GridObject<T>> _grid;
    private int _x;
    private int _y;
    private T _value;

    public GridObject(Grid2D<GridObject<T>> grid, int x, int y)
    {
        _grid = grid;
        _x = x;
        _y = y;
    }

    public T GetValue() => _value;

    public void SetValue(T value)
    {
        _value = value;
    }
}
