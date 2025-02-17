using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using Scripts.Ecs.Components;
using Scripts.DataModels;
using Scripts.GameView;

namespace Scripts.Ecs.Systems
{
    public class SpawnGemsSystem : BaseSystem<World, float>
    {
        private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>();
        private QueryDescription _spawnDesc = new QueryDescription().WithAll<GridComponent, SpawnGemsComponent>();
        public SpawnGemsSystem(World world, IObjectPool<Gem> gemPool, List<GemTypeSO> gemTypes) : base(world)
        {
            _gemPool = gemPool;
            _gemTypes = gemTypes;
        }

        //private Gem _gemPrefab;
        private IObjectPool<Gem> _gemPool;
        private IObjectPool<ExplosionVfx> _vfxPool;
        private List<GemTypeSO> _gemTypes;
        private GameObject _parent;

        public override void Initialize()
        {
            base.Initialize();
            _parent = new GameObject("Gems");
            // Run query, can also run multiple queries inside the update method
            World.Query(in _desc, (ref GridComponent grid) =>
            {
                for (int x = 0; x < grid.width; x++)
                {
                    for (int y = 0; y < grid.height; y++)
                    {
                        if (grid.mask[x, y] == 1)
                            CreateGem(ref grid, x, y, _gemTypes, _parent.transform);
                    }
                }
            });
        }

        public override void Update(in float deltaTime)
        {
            World.Query(in _spawnDesc, (Entity entity, ref GridComponent grid, ref SpawnGemsComponent spawn) =>
            {

                if (spawn.delayCounter < spawn.delay)
                {
                    spawn.delayCounter += Time.deltaTime;
                    return;
                }

                for (int x = 0; x < grid.width; x++)
                {
                    for (int y = 0; y < grid.height; y++)
                    {
                        if (grid.IsEmptyTile(x, y) && grid.mask[x, y] == 1)
                            CreateGem(ref grid, x, y, _gemTypes, _parent.transform);
                    }
                }

                entity.Remove<SpawnGemsComponent>();
                entity.Remove<BlockedComponent>();
            });
        }

        private void CreateGem(ref GridComponent grid, int x, int y, List<GemTypeSO> gemTypes, Transform parent)
        {
            Gem gem = _gemPool.Get();
            gem.transform.position = grid.coordinateConverter.GridToWorldCenter(x, y, grid.cellSize, grid.origin);
            gem.transform.DOScale(Vector3.zero, 0.2f).From().SetDelay(Random.Range(0, 0.2f));

            HashSet<GemTypeSO> forbidden = new();

            if (grid.IsGemTile(x - 2, y) && grid.IsGemTile(x - 1, y) &&
                grid.GetTileValue(x - 2, y).Get<GemComponent>().gem.GetGemType() == grid.GetTileValue(x - 1, y).Get<GemComponent>().gem.GetGemType())
            {
                forbidden.Add(grid.GetTileValue(x - 1, y).Get<GemComponent>().gem.GetGemType());
            }
            if (grid.IsGemTile(x - 1, y) && grid.IsGemTile(x + 1, y) &&
                grid.GetTileValue(x - 1, y).Get<GemComponent>().gem.GetGemType() == grid.GetTileValue(x + 1, y).Get<GemComponent>().gem.GetGemType())
            {
                forbidden.Add(grid.GetTileValue(x - 1, y).Get<GemComponent>().gem.GetGemType());
            }
            if (grid.IsGemTile(x + 1, y) && grid.IsGemTile(x + 2, y) &&
                grid.GetTileValue(x + 1, y).Get<GemComponent>().gem.GetGemType() == grid.GetTileValue(x + 2, y).Get<GemComponent>().gem.GetGemType())
            {
                forbidden.Add(grid.GetTileValue(x + 1, y).Get<GemComponent>().gem.GetGemType());
            }
            if (grid.IsGemTile(x, y - 1) && grid.IsGemTile(x, y - 2) &&
                grid.GetTileValue(x, y - 1).Get<GemComponent>().gem.GetGemType() == grid.GetTileValue(x, y - 2).Get<GemComponent>().gem.GetGemType())
            {
                forbidden.Add(grid.GetTileValue(x, y - 1).Get<GemComponent>().gem.GetGemType());
            }


            GemTypeSO[] allowedTypes = gemTypes.Except(forbidden).ToArray();
            if (allowedTypes.Length > 0)
            {
                gem.SetType(allowedTypes[Random.Range(0, allowedTypes.Length)]);
            }

            else
            {
                gem.SetType(gemTypes[Random.Range(0, gemTypes.Count)]);
            }

            var gemEntity = World.Create(new GemComponent(gem), new GridPositionComponent(new Vector2Int(x, y)));

            grid.gridArray[x, y] = gemEntity;
        }
    }
}
