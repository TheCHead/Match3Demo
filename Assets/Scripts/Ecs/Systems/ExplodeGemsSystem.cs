using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Scripts.Ecs.Components;
using Scripts.DataModels;
using Scripts.GameView;

namespace Scripts.Ecs.Systems
{
    public class ExplodeGemsSystem : BaseSystem<World, float>
    {
        private QueryDescription _queueDesc = new QueryDescription().WithAll<GridComponent, QueueExplosionComponent>();
        private QueryDescription _explodeDesc = new QueryDescription().WithAll<GridComponent, ExplodeGemsComponent>();
        public ExplodeGemsSystem(World world) : base(world) { }

        private float _explodeDelay = 0.1f;
        private Queue<MatchBatch> _explosionQueue = new();

        public override void Update(in float deltaTime)
        {
            World.Query(in _queueDesc, (Entity entity, ref QueueExplosionComponent explosion) =>
            {
                _explosionQueue.Enqueue(explosion.batch);
                entity.Remove<QueueExplosionComponent>();
            });

            World.Query(in _explodeDesc, (Entity entity, ref GridComponent grid, ref ExplodeGemsComponent explode) =>
            {
                if (explode.delayCounter < explode.delay)
                {
                    explode.delayCounter += Time.deltaTime;
                    return;
                }

                foreach (var batch in _explosionQueue)
                {
                    ExplodeBatch(entity, batch);
                }
                _explosionQueue.Clear();
                entity.Remove<ExplodeGemsComponent>();
                entity.Add(new GemFallComponent(0.3f));
            });
        }

        private void ExplodeBatch(Entity gridEnity, MatchBatch batch)
        {
            List<Vector2Int> matches = new List<Vector2Int>(batch.matches);
            var grid = gridEnity.Get<GridComponent>();

            foreach (var item in PassiveItems.Items)
            {
                item.OnExplodeBatch(gridEnity, batch);
            }

            for (int i = 0; i < matches.Count; i++)
            {
                if (grid.IsEmptyTile(matches[i].x, matches[i].y))
                    continue;

                var tile = grid.GetTileValue(matches[i].x, matches[i].y);

                if (tile.Has<GemComponent>())
                {
                    Gem gem = tile.Get<GemComponent>().gem;
                    DestroyGemAsync(gem, i * _explodeDelay);
                    tile.Remove<GemComponent>();
                }

                grid.SetTileValue(matches[i].x, matches[i].y, Entity.Null);
                World.Destroy(tile);
            }
        }

        private async void DestroyGemAsync(Gem gem, float delay)
        {
            await UniTask.Delay(Mathf.CeilToInt(delay * 1000));
            gem.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f, 1);
            await UniTask.Delay(100);
            gem.DestroyGem();
        }
    }
}

namespace Scripts.Ecs.Components
{
    public struct QueueExplosionComponent
    {
        public MatchBatch batch;

        public QueueExplosionComponent(MatchBatch batch)
        {
            this.batch = batch;
        }
    }
}

