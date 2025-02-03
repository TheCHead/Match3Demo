using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;

public partial class EcsEntry
{
    public class UnblockGridSystem : BaseSystem<World, float>
    {
        private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, BlockedComponent>().WithNone<SwapTilesComponent, SwapTilesProcessComponent, MatchGemsComponent, ExplodeGemsComponent, GemFallComponent, GemFallProcessComponent, SpawnGemsComponent>();
        public UnblockGridSystem(World world) : base(world) {}

        public override void Update(in float deltaTime)
        {
            World.Query(in _desc, (Entity entity) => {
                entity.Remove<BlockedComponent>();
            });
        }
    }
}
