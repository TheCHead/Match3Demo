using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Cysharp.Threading.Tasks;

public class TriggerDispatchSystem : BaseSystem<World, float>
{
    private QueryDescription _triggerDesc = new QueryDescription().WithAll<GridComponent, TriggerGemsComponent>();
    public TriggerDispatchSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _triggerDesc, (Entity entity, ref GridComponent grid, ref TriggerGemsComponent triggerComponent) => {
            TriggerMatchSetAsync(entity, triggerComponent.matchSet);
            entity.Add(new TriggerDispatchProcessComponent());
            entity.Remove<TriggerGemsComponent>();
        });
    }

    private async void TriggerMatchSetAsync(Entity entity, MatchSet set)
    {
        float triggerTime = 0.4f;
        foreach (var batch in set.batches)
        {
            entity.Add(new TriggerGemBatchComponent(batch, triggerTime));

            await UniTask.WaitForSeconds(triggerTime);
            triggerTime *= 0.95f;

            foreach (var item in PassiveItems.Items)
            {
                await item.OnTriggerBatch(entity, batch, triggerTime);
            }
        }

        entity.Remove<TriggerDispatchProcessComponent>();
        entity.Add(new ExplodeGemsComponent(0f));
    }
}

public struct TriggerGemsComponent
{
    public MatchSet matchSet;

    public TriggerGemsComponent(MatchSet matchSet)
    {
        this.matchSet = matchSet;
    }
}

public struct TriggerDispatchProcessComponent
{
    
}
