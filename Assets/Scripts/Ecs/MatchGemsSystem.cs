using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;
using static MatchPatterns;

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
                entity.Add(new TriggerGemsComponent(matchSet));
            }
            else
            {
                entity.Add(new ResetScoreComponent());
                entity.Add(new SpawnGemsComponent(0.3f));
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
            MatchBatch matchBatch = new(MatchType.Horizontal);

            for (int x = 0; x < grid.width - 2; x++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x + 1, y) || !grid.IsGemTile(x + 2, y)) 
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchType.Horizontal);
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
                    matchBatch = new(MatchType.Horizontal);
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
            MatchBatch matchBatch = new(MatchType.Vertical);

            for (int y = 0; y < grid.height - 2; y++)
            {
                if (!grid.IsGemTile(x, y) || !grid.IsGemTile(x, y + 1) || !grid.IsGemTile(x, y + 2)) 
                {
                    if (matchBatch.matches.Count > 0)
                    {
                        matchSet.batches.Add(matchBatch);
                    }
                    matchBatch = new(MatchType.Vertical);
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
                    matchBatch = new(MatchType.Vertical);
                }
            }
            if (matchBatch.matches.Count > 0)
            {
                matchSet.batches.Add(matchBatch);
            } 
        }

        // Match patterns
        List<MatchType> patternTypes = new();

        foreach (var item in PassiveItems.Items)
        {
            if (item.TryGetMatchType(out MatchType type))
            {
                patternTypes.Add(type);
            }
        }

        MatchSet patternSet = GetPatternMatches(ref grid, patternTypes);
        matchSet.batches.AddRange(patternSet.batches);

        return matchSet;
    }    

    private MatchSet GetPatternMatches(ref GridComponent grid, List<MatchType> types)
    {
        MatchSet patternSet = new();
        patternSet.batches = new();

        //var patternKeys = MatchPatterns.Patterns.Keys;
        bool match;

        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                foreach (var type in types)
                {
                    int[,] pattern = MatchPatterns.Patterns[type];
                    MatchBatch batch = new MatchBatch(type);
                    GemTypeSO matchType = null;
                    match = true;
                    if (!match)
                        continue;
                    for (int c = 0; c < pattern.GetLength(1); c++)
                    {
                        if (!match)
                            break;
                        for (int r = 0; r < pattern.GetLength(0); r++)
                        {
                            if (pattern[r, c] == 1)
                            {
                                if (grid.IsGemTile(x + c, y - r))
                                {
                                    GemTypeSO gemType = grid.GetTileValue(x + c, y - r).Get<GemComponent>().gem.GetGemType();
                                    matchType ??= gemType;
                                    if (matchType != gemType)
                                    {
                                        match = false;
                                        break;
                                    }
                                    batch.matches.Add(new Vector2Int(x + c, y - r));
                                }
                                else
                                {
                                    match = false; 
                                    break;
                                }
                            }
                        }
                    }
                    if (match && batch.matches.Count > 0)
                    {
                        patternSet.batches.Add(batch);
                    }
                }
            }
        }
        return patternSet;
    }
}

public struct MatchSet
{
    public List<MatchBatch> batches;
}

public struct MatchBatch
{
    public HashSet<Vector2Int> matches;
    public MatchType type;

    public MatchBatch(MatchType type)
    {
        this.type = type;
        matches = new();
    }
}
