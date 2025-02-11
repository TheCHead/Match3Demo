using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Cysharp.Threading.Tasks;
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

public interface IPassiveItem
{
    public bool TryGetMatchType(out MatchType matchType);
    public UniTask OnTriggerBatch(Entity entity, MatchBatch batch, float timeframe);
}
