using Arch.Core;
using Arch.System;
using UnityEngine;

public class InputSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>().WithNone<BlockedComponent>();
    public InputSystem(World world, InputReader reader) : base(world) {
        _inputReader = reader;
    }
    private InputReader _inputReader;


    public override void Update(in float deltaTime)
    {
        World.Query(in _desc, (ref GridComponent grid) => {
            if (_inputReader.IsFireTriggered)
            {
                var gridPos = grid.coordinateConverter.WorldToGrid(Camera.main.ScreenToWorldPoint(_inputReader.Selected), grid.cellSize, grid.origin);

                if (grid.IsObjectTile(gridPos.x, gridPos.y))
                {
                    grid.tileSelection.Add(gridPos);
                }
            }
        });  
    }
}
