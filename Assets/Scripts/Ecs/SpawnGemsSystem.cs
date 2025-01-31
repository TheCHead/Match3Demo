using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;

public class SpawnGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>();
    public SpawnGemsSystem(World world, Gem gemPrefab, List<GemTypeSO> gemTypes) : base(world) {_gemPrefab = gemPrefab; _gemTypes = gemTypes;}

    private Gem _gemPrefab;
    private List<GemTypeSO> _gemTypes;

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
                        CreateGem(ref grid, x, y, _gemTypes, parent.transform);
                }
            }
        });  
    }
    
    private void CreateGem(ref GridComponent grid, int x, int y, List<GemTypeSO> gemTypes, Transform parent)
    {        
        Gem gem = GameObject.Instantiate(_gemPrefab, grid.coordinateConverter.GridToWorldCenter(x, y, grid.cellSize, grid.origin), Quaternion.identity, parent);

        List<GemTypeSO> forbiddenTypes = new();
        
        if  (grid.IsValidTile(x - 1, y) && !grid.IsEmptyTile(x - 1, y))
        {
            forbiddenTypes.Add(grid.GetTileValue(x - 1, y).Get<GemComponent>().gem.GetGemType());
        }
        if  (grid.IsValidTile(x, y - 1) && !grid.IsEmptyTile(x, y - 1))
        {
            forbiddenTypes.Add(grid.GetTileValue(x, y - 1).Get<GemComponent>().gem.GetGemType());
        }
        GemTypeSO[] allowedTypes = gemTypes.Except(forbiddenTypes).ToArray();
        gem.SetType(allowedTypes[Random.Range(0, allowedTypes.Length)]);

        var gemEntity = World.Create(new GemComponent(gem), new GridPositionComponent(new Vector2Int(x, y)));

        grid.gridArray[x, y] = gemEntity;
    }
}
