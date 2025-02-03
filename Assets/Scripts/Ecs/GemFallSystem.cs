using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;
using UnityEngine;

public class GemFallSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, GemFallComponent>();
    private QueryDescription _fallProcessDesc = new QueryDescription().WithAll<GridComponent, GemFallProcessComponent>();

    public GemFallSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _desc, (Entity entity, ref GridComponent grid, ref GemFallComponent fall) => {
            if (fall.delayCounter < fall.delay)
            {
                fall.delayCounter += Time.deltaTime;
                return;
            }

            entity.Add(new GemFallProcessComponent(MakeGemsFall(ref grid)));
            entity.Remove<GemFallComponent>();
        });

        World.Query(in _fallProcessDesc, (Entity entity, ref GemFallProcessComponent fallProcess) => {
            if (!fallProcess.sequence.IsActive() || fallProcess.sequence.IsComplete())
            {
                fallProcess.sequence.Kill();
                entity.Remove<GemFallProcessComponent>();
                entity.Add(new MatchGemsComponent());
            }
        });
    }

    private Sequence MakeGemsFall(ref GridComponent grid)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetAutoKill(false);
        float delay = 0f;

        for (var x = 0; x < grid.width; x++) 
        {
            delay = 0f;
            for (var y = 0; y < grid.height; y++) 
            {
                if (grid.IsEmptyTile(x, y)) 
                {
                    for (var i = y + 1; i < grid.height; i++) 
                    {
                        if (!grid.IsValidTile(x, i))
                        {
                            break;
                        }

                        if (grid.IsGemTile(x, i))
                        {
                            var gem = grid.GetTileValue(x, i);
                            grid.SetTileValue(x, y, grid.GetTileValue(x, i));
                            grid.SetTileValue(x, i, Entity.Null);
                            sequence.Join(
                            gem.Get<GemComponent>().gem.transform
                                .DOLocalMove(grid.coordinateConverter.GridToWorldCenter(x, y, grid.cellSize, grid.origin), 0.75f)
                                .SetEase(Ease.OutBounce)
                                .SetDelay(delay));
                            delay += 0.02f;
                            break;
                        }
                    }
                }
            }
        }
        
        return sequence;
    }
}

public struct GemFallProcessComponent
{
    public Sequence sequence;

    public GemFallProcessComponent(Sequence sequence)
    {
        this.sequence = sequence;
    }
}
