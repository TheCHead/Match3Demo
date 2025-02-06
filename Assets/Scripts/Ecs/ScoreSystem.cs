using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static MatchPatterns;

public class ScoreSystem : BaseSystem<World, float>
{
    private QueryDescription _scoreMatchSetDesc = new QueryDescription().WithAll<GridComponent, ScoreMatchSetComponent>();
        private QueryDescription _finilizeDesc = new QueryDescription().WithAll<GridComponent, ResetScoreComponent>();

    public ScoreSystem(World world, ScoreScreen screen) : base(world) { _screen = screen; }

    private ScoreScreen _screen;

    public override void Update(in float deltaTime)
    {
        World.Query(in _scoreMatchSetDesc, (Entity entity, ref GridComponent grid, ref ScoreMatchSetComponent matchSetComponent) => {
            ScoreMatchSetAsync(entity, matchSetComponent.matchSet);
            entity.Add(new ScoreProcessComponent());
            entity.Remove<ScoreMatchSetComponent>();
        });

        World.Query(in _finilizeDesc, (Entity entity, ref GridComponent grid, ref ResetScoreComponent finalize) => {
            _screen.Reset();
            entity.Remove<ResetScoreComponent>();
        });
    }

    private async void ScoreMatchSetAsync(Entity entity, MatchSet set)
    {
        int delay = 400;
        foreach (var batch in set.batches)
        {
            Debug.Log($"Scored {Enum.GetName(typeof(MatchType), batch.type)}");

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

            if (entity.Has<GridComponent>())
            {
                foreach (var match in batch.matches)
                {
                    entity.Get<GridComponent>().GetTileValue(match).Get<GemComponent>().gem.Highlight(delay * 0.001f);
                }
            }

            _screen.AddScore(points, mult);
            await UniTask.Delay(delay);
            delay = Mathf.RoundToInt(delay * 0.9f);
        }
        await UniTask.Delay(300);

        _screen.UpdateTotal();
        entity.Remove<ScoreProcessComponent>();
        entity.Add(new ExplodeGemsComponent(set, 0f));
    }
}

public struct ScoreMatchSetComponent
{
    public MatchSet matchSet;

    public ScoreMatchSetComponent(MatchSet matchSet)
    {
        this.matchSet = matchSet;
    }
}

public struct ScoreProcessComponent
{

}

public struct ResetScoreComponent
{

}
