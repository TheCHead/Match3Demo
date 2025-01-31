using Arch.Core;
using Arch.System;
using UnityEngine;

public class InputSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>();
    public InputSystem(World world, InputReader reader) : base(world) {
        _inputReader = reader;
        _inputReader.FireEvent += OnFireEvent;
    }
    private InputReader _inputReader;

    private void OnFireEvent()
    {
        World.Query(in _desc, (ref GridComponent grid) => {
            var gridPos = grid.coordinateConverter.WorldToGrid(Camera.main.ScreenToWorldPoint(_inputReader.Selected), grid.cellSize, grid.origin);

            if (grid.IsObjectTile(gridPos.x, gridPos.y))
            {
                grid.tileSelection.Add(gridPos);
            }
        });  
    }

    public override void Dispose()
    {
        base.Dispose();
        _inputReader.FireEvent -= OnFireEvent;
    }
}
