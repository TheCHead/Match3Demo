using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;

public class SwapTilesSystem : BaseSystem<World, float>
{
    private QueryDescription _swapDesc = new QueryDescription().WithAll<GridComponent, SwapTilesComponent>();
    private QueryDescription _swapProcessDesc = new QueryDescription().WithAll<GridComponent, SwapTilesProcessComponent>();
    public SwapTilesSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _swapDesc, (Entity entity, ref GridComponent grid, ref SwapTilesComponent swap) => {
            
            if (!grid.IsGemTile(swap.tileA) || !grid.IsGemTile(swap.tileB))
            {
                entity.Remove<SwapTilesComponent>();
            }

            var tileA = grid.GetTileValue(swap.tileA);
            var tileB = grid.GetTileValue(swap.tileB);

            Sequence swapSequence = DOTween.Sequence();
                swapSequence.SetAutoKill(false);

                swapSequence.Append(tileA.Get<GemComponent>().gem.transform
                .DOLocalMove(grid.coordinateConverter.GridToWorldCenter(swap.tileB.x, swap.tileB.y, grid.cellSize, grid.origin), 0.3f)
                .SetEase(Ease.InSine));

                swapSequence.Join(tileB.Get<GemComponent>().gem.transform
                .DOLocalMove(grid.coordinateConverter.GridToWorldCenter(swap.tileA.x, swap.tileA.y, grid.cellSize, grid.origin), 0.3f)
                .SetEase(Ease.InSine));

                entity.Add(new SwapTilesProcessComponent(swapSequence));

                grid.SetTileValue(swap.tileA.x, swap.tileA.y, tileB);
                grid.SetTileValue(swap.tileB.x, swap.tileB.y, tileA);

            entity.Remove<SwapTilesComponent>();
        });  

        World.Query(in _swapProcessDesc, (Entity entity, ref GridComponent grid, ref SwapTilesProcessComponent swapProcess) => {
            if (swapProcess.sequence.IsComplete())
            {
                swapProcess.sequence.Kill();
                entity.Remove<SwapTilesProcessComponent>();
                entity.Add(new MatchGemsComponent());
            }
        });
    }
}
