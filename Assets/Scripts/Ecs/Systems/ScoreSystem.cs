using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Scripts.DataModels;
using Scripts.Ecs.Components;
using Scripts.UI;
using Scripts.UI.Presenters;
using static MatchPatterns;

namespace Scripts.Ecs.Systems
{
    public class ScoreSystem : BaseSystem<World, float>
    {
        private QueryDescription _scoreBatchDesc = new QueryDescription().WithAll<ScoreBatchComponent>();
        private QueryDescription _finilizeDesc = new QueryDescription().WithAll<GridComponent, ResetScoreComponent>();
        private IScorePresenter _scorePresenter;
        private UIService _uiService;
        public ScoreSystem(World world, UIService uIService) : base(world) 
        { 
            _uiService = uIService;
            _scorePresenter = _uiService.GetPresenter(EUIScreen.Score) as IScorePresenter;
        }

        public override void Update(in float deltaTime)
        {
            CommandBuffer commandBuffer = new CommandBuffer();
            World.Query(in _scoreBatchDesc, (Entity entity, ref ScoreBatchComponent batchComponent) =>
            {
                ScoreBatch(batchComponent.batch);
                commandBuffer.Destroy(entity);
            });

            World.Query(in _finilizeDesc, (Entity entity, ref GridComponent grid, ref ResetScoreComponent finalize) =>
            {
                _scorePresenter.UpdateTotal();
                entity.Remove<ResetScoreComponent>();
            });

            commandBuffer.Playback(World, true);
        }

        private void ScoreBatch(MatchBatch batch)
        {
            float points = 0;
            float mult = 0;
            switch (batch.type)
            {
                case MatchType.None:
                    points = 3f * batch.matches.Count;
                    break;
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

            _scorePresenter.OnScore(points, mult);
        }
    }
}

namespace Scripts.Ecs.Components
{
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
}
