using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;

public class TriggerGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _triggerDesc = new QueryDescription().WithAll<GridComponent, TriggerGemBatchComponent>();
    public TriggerGemsSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _triggerDesc, (Entity entity, ref GridComponent grid, ref TriggerGemBatchComponent gemBatchComponent) => {
            foreach (var match in gemBatchComponent.batch.matches)
            {
                grid.GetTileValue(match).Get<GemComponent>().gem.Highlight(gemBatchComponent.timeframe);
                entity.Remove<TriggerGemBatchComponent>();
            }
            World.Create(new ScoreBatchComponent(gemBatchComponent.batch));
        });
    }
}

public struct TriggerGemBatchComponent
{
    public MatchBatch batch;
    public float timeframe;

    public TriggerGemBatchComponent(MatchBatch batch, float timeframe)
    {
        this.batch = batch;
        this.timeframe = timeframe;
    }
}
