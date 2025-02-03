using Arch.Core;
using Arch.System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InitializeGridSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, TilemapComponent>();
    public InitializeGridSystem(World world) : base(world) {}

    public override void Initialize()
    {
        base.Initialize();

        World.Query(in _desc, (ref GridComponent grid, ref TilemapComponent tilemap) => {
            
            tilemap.tilemap.CompressBounds();

            grid.width = tilemap.tilemap.size.x;
            grid.height = tilemap.tilemap.size.y;
            grid.cellSize = tilemap.tilemap.cellSize.x;
            grid.origin = tilemap.tilemap.origin;
            grid.gridArray = new Entity[grid.width, grid.height];
            grid.mask = GetTilemapMask(tilemap.tilemap);
            grid.coordinateConverter = new VerticalConverter();
            grid.tileSelection = new();
        });  
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
}
