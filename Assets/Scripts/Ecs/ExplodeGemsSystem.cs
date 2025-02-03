using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ExplodeGemsSystem : BaseSystem<World, float>
{
    private QueryDescription _swapDesc = new QueryDescription().WithAll<GridComponent, ExplodeGemsComponent>();
    public ExplodeGemsSystem(World world) : base(world) {}

    private float _exlodeDelay = 0.1f;

    public override void Update(in float deltaTime)
    {
        World.Query(in _swapDesc, (Entity entity, ref GridComponent grid, ref ExplodeGemsComponent explode) => {
            if (explode.delayCounter < explode.delay)
            {
                explode.delayCounter += Time.deltaTime;
                return;
            }
            
            ExplodeGems(ref grid, explode.gems);
            entity.Remove<ExplodeGemsComponent>();
            entity.Add(new GemFallComponent(explode.gems.Count * _exlodeDelay + 0.3f));
        });
    }

 
    private void ExplodeGems(ref GridComponent grid, List<Vector2Int> matches)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            var tile = grid.GetTileValue(matches[i].x, matches[i].y);

            if (tile.Has<GemComponent>())
            {
                Gem gem = tile.Get<GemComponent>().gem;
                DestroyGemAsync(gem, i * _exlodeDelay);
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
