using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using static MatchPatterns;

public class ScoreSystem : BaseSystem<World, float>
{
    private QueryDescription _scoreBatchDesc = new QueryDescription().WithAll<ScoreBatchComponent>();
 
    private QueryDescription _finilizeDesc = new QueryDescription().WithAll<GridComponent, ResetScoreComponent>();

    public ScoreSystem(World world, ScoreScreen screen) : base(world) { _screen = screen; }

    private ScoreScreen _screen;

    public override void Update(in float deltaTime)
    {
        CommandBuffer commandBuffer = new CommandBuffer();
        World.Query(in _scoreBatchDesc, (Entity entity, ref ScoreBatchComponent batchComponent) => {
            ScoreBatch(batchComponent.batch);
            commandBuffer.Destroy(entity);
        });

        World.Query(in _finilizeDesc, (Entity entity, ref GridComponent grid, ref ResetScoreComponent finalize) => {
            _screen.UpdateTotal();
            _screen.Reset();
            entity.Remove<ResetScoreComponent>();
        });

        commandBuffer.Playback(World, true);
    }

    private void ScoreBatch(MatchBatch batch)
    {
        float points = 0;
        float mult = 0;
        switch  (batch.type)
        {
            case MatchType.Horizontal:
            case MatchType.Vertical:
                points = 3f * batch.matches.Count;
                mult = 3;
                break;
            case MatchType.Square:
                points = 4f * batch.matches.Count;
                mult = 3;
                break;
            case MatchType.Clover:
                points = 4f * batch.matches.Count;
                mult = 4;
                break;
            case MatchType.Tower:
                points = 5f * batch.matches.Count;
                mult = 4;
                break;
            case MatchType.Holy:
                points = 5f * batch.matches.Count;
                mult = 5;
                break;
        }

        _screen.AddScore(points, mult);
    }
}

public struct ScoreBatchComponent
{
    public MatchBatch batch;
    public ScoreBatchComponent(MatchBatch batch)
    {
        this.batch = batch;
    }
}

public struct ResetScoreComponent
{

}
