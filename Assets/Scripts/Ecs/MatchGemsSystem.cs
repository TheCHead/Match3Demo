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

            MatchSet matchSet = FindMatches(ref grid);

            if (matchSet.batches.Count > 0)
            {
                entity.Add(new ExplodeGemsComponent(matchSet, 0.1f));
            }
            else
            {
                World.Create(new FinalizeScoreComponent());
                entity.Add(new SpawnGemsComponent(0.5f));
            }
            entity.Remove<MatchGemsComponent>();
        });
    }

    private MatchSet FindMatches(ref GridComponent grid)
    {
        MatchSet matchSet = new();
        matchSet.batches = new();

        // Horizontal
        for (int y = 0; y < grid.height; y++)
        {
            MatchBatch matchBatch = new(MatchBatch.MatchType.Horizontal);

            for (int x = 0; x < grid.width - 2; x++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x + 1, y) || !grid.IsGemTile(x + 2, y)) 
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchBatch.MatchType.Horizontal);
                    continue;
                };

                var gemA = grid.GetTileValue(x, y);
                var gemB = grid.GetTileValue(x + 1, y);
                var gemC = grid.GetTileValue(x + 2, y);

                if (gemA.Get<GemComponent>().gem.GetGemType() == gemB.Get<GemComponent>().gem.GetGemType() && gemB.Get<GemComponent>().gem.GetGemType() == gemC.Get<GemComponent>().gem.GetGemType())
                {
                    matchBatch.matches.Add(new Vector2Int(x, y));
                    matchBatch.matches.Add(new Vector2Int(x + 1, y));
                    matchBatch.matches.Add(new Vector2Int(x + 2, y));
                }
                else
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchBatch.MatchType.Horizontal);
                }
            }

            if (matchBatch.matches.Count > 0)
            {
                matchSet.batches.Add(matchBatch);
            } 
        }

        // Vertical
        for (int x = 0; x < grid.width; x++)
        {
            MatchBatch matchBatch = new(MatchBatch.MatchType.Vertical);

            for (int y = 0; y < grid.height - 2; y++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x, y + 1) || !grid.IsGemTile(x, y + 2)) 
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchBatch.MatchType.Vertical);
                    continue;
                };

                var gemA = grid.GetTileValue(x, y);
                var gemB = grid.GetTileValue(x, y + 1);
                var gemC = grid.GetTileValue(x, y + 2);

                if (gemA.Get<GemComponent>().gem.GetGemType() == gemB.Get<GemComponent>().gem.GetGemType() && gemB.Get<GemComponent>().gem.GetGemType() == gemC.Get<GemComponent>().gem.GetGemType())
                {
                    matchBatch.matches.Add(new Vector2Int(x, y));
                    matchBatch.matches.Add(new Vector2Int(x, y + 1));
                    matchBatch.matches.Add(new Vector2Int(x, y + 2));
                }
                else
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchBatch.MatchType.Vertical);
                }
            }
            if (matchBatch.matches.Count > 0)
            {
                matchSet.batches.Add(matchBatch);
            } 
        }

        return matchSet;
    }    
}

public struct MatchSet
{
    public List<MatchBatch> batches;
}

public struct MatchBatch
{
    public enum MatchType { Horizontal, Vertical};
    public HashSet<Vector2Int> matches;
    public MatchType type;

    public MatchBatch(MatchType type)
    {
        this.type = type;
        matches = new();
    }
}



