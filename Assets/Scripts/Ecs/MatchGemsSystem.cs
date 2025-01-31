using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;

public class MatchGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _matchDesc = new QueryDescription().WithAll<GridComponent, MatchGemsComponent>();

    public MatchGemsSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _matchDesc, (Entity entity, ref GridComponent grid) => {
            List<Vector2Int> matches = FindMatches(ref grid);
            if (matches.Count > 0)
            {
                entity.Add(new ExplodeGemsComponent(matches));
            }
            entity.Remove<MatchGemsComponent>();
        });
    }

    private List<Vector2Int> FindMatches(ref GridComponent grid)
    {
        HashSet<Vector2Int> matches = new();

        // Horizontal
        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.width - 2; x++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x + 1, y) || !grid.IsGemTile(x + 2, y)) continue;

                var gemA = grid.GetTileValue(x, y);
                var gemB = grid.GetTileValue(x + 1, y);
                var gemC = grid.GetTileValue(x + 2, y);

                //if (!gemA.Has<GemComponent>() || !gemB.Has<GemComponent>() || !gemC.Has<GemComponent>()) continue;

                if (gemA.Get<GemComponent>().gem.GetGemType() == gemB.Get<GemComponent>().gem.GetGemType() && gemB.Get<GemComponent>().gem.GetGemType() == gemC.Get<GemComponent>().gem.GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        // Vertical
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height - 2; y++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x, y + 1) || !grid.IsGemTile(x, y + 2)) continue;

                var gemA = grid.GetTileValue(x, y);
                var gemB = grid.GetTileValue(x, y + 1);
                var gemC = grid.GetTileValue(x, y + 2);

                //if (!gemA.Has<GemComponent>() || !gemB.Has<GemComponent>() || !gemC.Has<GemComponent>()) continue;

                if (gemA.Get<GemComponent>().gem.GetGemType() == gemB.Get<GemComponent>().gem.GetGemType() && gemB.Get<GemComponent>().gem.GetGemType() == gemC.Get<GemComponent>().gem.GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        if (matches.Count == 0)
        {
            //_audioManager.PlayeNoMatch();
        }
        else
        {
            //_audioManager.PlayMatch();
        }

        return new List<Vector2Int>(matches);
    }    
}
