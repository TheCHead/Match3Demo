using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arch.Core;
using Arch.Core.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static MatchPatterns;

public static class PassiveItems
{
    public static List<IPassiveItem> Items = new();
}

public class SquarePeg : IPassiveItem
{
    public async UniTask OnExplodeBatch(Entity entity, MatchBatch batch)
    {
        await UniTask.WaitForEndOfFrame();
    }

    public async UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime)
    {
        // retrigger square batch
        if (batch.type == MatchType.Square)
        {
            entity.Add(new TriggerGemBatchComponent(batch, triggerTime));
            await UniTask.WaitForSeconds(triggerTime);
        } 
    }

    public bool TryGetMatchType(out MatchType matchType)
    {
        matchType = MatchType.Square;
        return true;
    }
}

public class Clover : IPassiveItem
{
    public async UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime)
    {
        // trigger all tiles horizontally and vertically from clover center
        if (batch.type == MatchType.Clover)
        {
            if (entity.Has<GridComponent>())
            {
                var grid = entity.Get<GridComponent>();
                MatchBatch newBatch = new MatchBatch(MatchType.None, null);
                Vector2Int top = batch.matches.OrderByDescending(match => match.y).First();
                Vector2Int center = top + Vector2Int.down;

                for (int x = center.x + 2; x < grid.width; x++)
                {
                    Vector2Int tile = new Vector2Int(x, center.y);
                    if (grid.IsGemTile(tile))
                    {
                        newBatch.matches.Add(tile);
                    }
                }

                for (int x = center.x - 2; x >= 0; x--)
                {
                    Vector2Int tile = new Vector2Int(x, center.y);
                    if (grid.IsGemTile(tile))
                    {
                        newBatch.matches.Add(tile);
                    }
                }

                for (int y = center.y + 2; y < grid.height; y++)
                {
                    Vector2Int tile = new Vector2Int(center.x, y);
                    if (grid.IsGemTile(tile))
                    {
                        newBatch.matches.Add(tile);
                    }
                }

                for (int y = center.y - 2; y >= 0; y--)
                {
                    Vector2Int tile = new Vector2Int(center.x, y);
                    if (grid.IsGemTile(tile))
                    {
                        newBatch.matches.Add(tile);
                    }
                }

                if (newBatch.matches.Count > 0)
                {
                    entity.Add(new TriggerGemBatchComponent(newBatch, triggerTime));
                    await UniTask.WaitForSeconds(triggerTime); 
                }
            }
        }
    }

    public async UniTask OnExplodeBatch(Entity entity, MatchBatch batch)
    {
        await UniTask.WaitForEndOfFrame();
    }

    public bool TryGetMatchType(out MatchType matchType)
    {
        matchType = MatchType.Clover;
        return true;
    }
}

public class Tower : IPassiveItem
{
    public async UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime)
    {
        // trigger all tiles of the same type
        if (batch.type == MatchType.Tower)
        {
            if (entity.Has<GridComponent>())
            {
                var grid = entity.Get<GridComponent>();
                MatchBatch newBatch = new MatchBatch(MatchType.None, batch.gemType);

                for (int x = 0; x < grid.width; x++)
                {
                    for (int y = 0; y < grid.height; y++)
                    {
                        Vector2Int tile = new Vector2Int(x, y);
                        if (grid.IsGemTile(tile) && !batch.matches.Contains(tile) && grid.GetTileValue(tile).Get<GemComponent>().gem.GetGemType() == batch.gemType)
                        {
                            newBatch.matches.Add(tile);
                        }
                    }
                }

                if (newBatch.matches.Count > 0)
                {
                    entity.Add(new TriggerGemBatchComponent(newBatch, triggerTime));
                    await UniTask.WaitForSeconds(triggerTime); 
                }
            }
        }
    }

    public async UniTask OnExplodeBatch(Entity entity, MatchBatch batch)
    {
        await UniTask.WaitForEndOfFrame();
    }

    public bool TryGetMatchType(out MatchType matchType)
    {
        matchType = MatchType.Tower;
        return true;
    }
}

public class Holy : IPassiveItem
{
    public async UniTask OnExplodeBatch(Entity gridEntity, MatchBatch batch)
    {
        if (batch.type == MatchType.Holy)
        {
            var grid = gridEntity.Get<GridComponent>();

            foreach (var match in batch.matches)
            {
                if (grid.TryGetGemValue(match + Vector2Int.left, out Gem gemL))
                {
                    gemL.SetType(batch.gemType);
                }
                if (grid.TryGetGemValue(match + Vector2Int.up, out Gem gemU))
                {
                    gemU.SetType(batch.gemType);
                }
                if (grid.TryGetGemValue(match + Vector2Int.right, out Gem gemR))
                {
                    gemR.SetType(batch.gemType);
                }
                if (grid.TryGetGemValue(match + Vector2Int.down, out Gem gemD))
                {
                    gemD.SetType(batch.gemType);
                }

                await UniTask.WaitForSeconds(0.1f);
            }
        }
    }

    public async UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime)
    {
        await UniTask.WaitForEndOfFrame();
    }

    public bool TryGetMatchType(out MatchType matchType)
    {
        matchType = MatchType.Holy;
        return true;
    }
}

public interface IPassiveItem
{
    public bool TryGetMatchType(out MatchType matchType);
    public UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime);
    public UniTask OnExplodeBatch(Entity entity, MatchBatch batch);
}
