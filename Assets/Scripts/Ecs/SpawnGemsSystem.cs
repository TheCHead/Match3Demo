using Arch.Core;
using Arch.System;
using UnityEngine;

public class SpawnGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>();
    public SpawnGemsSystem(World world, Gem gemPrefab) : base(world) {_gemPrefab = gemPrefab; }

    private Gem _gemPrefab;

    public override void Initialize()
    {
        base.Initialize();
        GameObject parent = new GameObject("Gems");
        // Run query, can also run multiple queries inside the update method
        World.Query(in _desc, (ref GridComponent grid) => {
            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    if (grid.mask[x, y] == 1)
                        CreateGem(ref grid, x, y, parent.transform);
                }
            }
        });  
    }
    
    private void CreateGem(ref GridComponent grid, int x, int y, Transform parent)
    {
        Gem gem = GameObject.Instantiate(_gemPrefab, grid.coordinateConverter.GridToWorldCenter(x, y, grid.cellSize, grid.origin), Quaternion.identity, parent);

        grid.gridArray[x, y] = gem;
    }
}
