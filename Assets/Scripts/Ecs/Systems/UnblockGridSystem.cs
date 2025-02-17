using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Scripts.Ecs.Components;

namespace Scripts.Ecs.Systems
{
    public class UnblockGridSystem : BaseSystem<World, float>
    {
        private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, BlockedComponent>().WithNone<SwapTilesComponent, SwapTilesProcessComponent, MatchGemsComponent, TriggerDispatchProcessComponent, ExplodeGemsComponent, GemFallComponent, GemFallProcessComponent, SpawnGemsComponent>();
        public UnblockGridSystem(World world) : base(world) { }

        public override void Update(in float deltaTime)
        {
            World.Query(in _desc, (Entity entity) =>
            {
                entity.Remove<BlockedComponent>();
            });
        }
    }
}

