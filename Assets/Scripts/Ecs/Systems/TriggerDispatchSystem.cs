using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Cysharp.Threading.Tasks;
using Scripts.DataModels;
using Scripts.Ecs.Components;

namespace Scripts.Ecs.Systems
{
    public class TriggerDispatchSystem : BaseSystem<World, float>
    {
        private QueryDescription _triggerDesc = new QueryDescription().WithAll<GridComponent, TriggerGemsComponent>();
        public TriggerDispatchSystem(World world) : base(world) { }

        public override void Update(in float deltaTime)
        {
            World.Query(in _triggerDesc, (Entity entity, ref GridComponent grid, ref TriggerGemsComponent triggerComponent) =>
            {
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
                triggerTime *= 0.9f;

                foreach (var item in PassiveItems.Items)
                {
                    await item.OnTriggerBatch(entity, batch, triggerTime);
                }
            }

            entity.Remove<TriggerDispatchProcessComponent>();
            entity.Add(new ExplodeGemsComponent(0f));
        }
    }


}

namespace Scripts.Ecs.Components
{
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
}
