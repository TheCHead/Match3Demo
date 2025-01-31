using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;
using UnityEngine;

public class ExplodeGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _swapDesc = new QueryDescription().WithAll<GridComponent, ExplodeGemsComponent>();
    public ExplodeGemsSystem(World world, ParticleSystem explosionVfx) : base(world) { _vfx = explosionVfx; }

    private ParticleSystem _vfx;

    public override void Update(in float deltaTime)
    {
        World.Query(in _swapDesc, (Entity entity, ref GridComponent grid, ref ExplodeGemsComponent explode) => {
            ExplodeGems(ref grid, explode.gems);
            entity.Remove<ExplodeGemsComponent>();
        });
    }

    private void ExplodeGems(ref GridComponent grid, List<Vector2Int> matches)
    {
        foreach (var match in matches)
        {
            var tile = grid.GetTileValue(match.x, match.y);

            if (tile.Has<GemComponent>())
            {
                Gem gem = tile.Get<GemComponent>().gem;
                gem.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f, 1);
                gem.DestroyGem();
                tile.Remove<GemComponent>();
            }

            ExplodeVFX(ref grid, match);
            grid.SetTileValue(match.x, match.y, Entity.Null);
            World.Destroy(tile);
        }
    }

    private void ExplodeVFX(ref GridComponent grid, Vector2Int match)
    {
        // TODO: pool
        var fx = GameObject.Instantiate(_vfx);
        fx.transform.position = grid.coordinateConverter.GridToWorldCenter(match.x, match.y, grid.cellSize, grid.origin);
        GameObject.Destroy(fx.gameObject, 2f);
    }
}
