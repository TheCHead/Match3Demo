using System.Collections.Generic;
using System.Linq;
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
    public async UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime)
    {
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
        if (batch.type == MatchType.Clover)
        {
            // make new batch and trigger it
            if (entity.Has<GridComponent>())
            {
                var grid = entity.Get<GridComponent>();
                MatchBatch newBatch = new MatchBatch(MatchType.None);
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

    public bool TryGetMatchType(out MatchType matchType)
    {
        matchType = MatchType.Clover;
        return true;
    }
}

public interface IPassiveItem
{
    public bool TryGetMatchType(out MatchType matchType);
    public UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float triggerTime);
}
