using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using DG.Tweening;

public class SwapTilesSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, SwapTilesComponent>();
    public SwapTilesSystem(World world) : base(world) {}

    public override void Update(in float deltaTime)
    {
        World.Query(in _desc, (Entity entity, ref GridComponent grid, ref SwapTilesComponent swap) => {
            var tileA = grid.GetTileValue(swap.tileA);
            var tileB = grid.GetTileValue(swap.tileB);

            if (tileA.Has<GemComponent>() && tileB.Has<GemComponent>())
            {
                tileA.Get<GemComponent>().gem.transform
                .DOLocalMove(grid.coordinateConverter.GridToWorldCenter(swap.tileB.x, swap.tileB.y, grid.cellSize, grid.origin), 0.5f)
                .SetEase(Ease.InSine);

                tileB.Get<GemComponent>().gem.transform
                .DOLocalMove(grid.coordinateConverter.GridToWorldCenter(swap.tileA.x, swap.tileA.y, grid.cellSize, grid.origin), 0.5f)
                .SetEase(Ease.InSine);

                grid.SetTileValue(swap.tileA.x, swap.tileA.y, tileB);
                grid.SetTileValue(swap.tileB.x, swap.tileB.y, tileA);
            }

            entity.Remove<SwapTilesComponent>();
        });  
    }
}
