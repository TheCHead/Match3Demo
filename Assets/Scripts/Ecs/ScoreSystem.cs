using System;
using Arch.Buffer;
using Arch.Core;
using Arch.System;
using UnityEngine;

public class ScoreSystem : BaseSystem<World, float>
{
    private QueryDescription _scoreDesc = new QueryDescription().WithAll<ScoreBatchComponent>();
    private QueryDescription _resetDesc = new QueryDescription().WithAll<FinalizeScoreComponent>();
    public ScoreSystem(World world, ScoreScreen screen) : base(world) { _screen = screen; }

    private ScoreScreen _screen;

    public override void Update(in float deltaTime)
    {

        var commandBuffer = new CommandBuffer();
        World.Query(in _scoreDesc, (Entity entity, ref ScoreBatchComponent scoreBatch) => {
            
            Debug.Log($"Scored {Enum.GetName(typeof(MatchType), scoreBatch.batch.type)}");

            float scorePoints = 5f * scoreBatch.batch.matches.Count;
            float scoreMultiplier = scoreBatch.batch.matches.Count;

            _screen.AddScore(scorePoints, scoreMultiplier);

            commandBuffer.Destroy(entity);
        });


        World.Query(in _resetDesc, (Entity entity) => {

            _screen.Finalize();
            commandBuffer.Destroy(entity);
        });

        commandBuffer.Playback(World, true);
    }
}

public struct FinalizeScoreComponent
{

}


