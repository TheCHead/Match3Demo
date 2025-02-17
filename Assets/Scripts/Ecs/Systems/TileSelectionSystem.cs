using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Scripts.Ecs.Components;
using UnityEngine;

namespace Scripts.Ecs.Systems
{
    public class TileSelectionSystem : BaseSystem<World, float>
    {
        private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>().WithNone<SwapTilesProcessComponent>();
        public TileSelectionSystem(World world) : base(world) { }

        public override void Update(in float deltaTime)
        {
            World.Query(in _desc, (Entity entity, ref GridComponent grid) =>
            {
                if (grid.tileSelection.Count == 2)
                {
                    Vector2Int tileA = grid.tileSelection[0];
                    Vector2Int tileB = grid.tileSelection[1];
                    if (!tileA.Equals(tileB))
                    {
                        entity.Add(new SwapTilesComponent(tileA, tileB), new BlockedComponent());
                    }

                    DeselectGridTiles(ref grid);
                    grid.tileSelection.Clear();
                }
            });
        }

        private void DeselectGridTiles(ref GridComponent grid)
        {
            foreach (var tile in grid.tileSelection)
            {
                if (grid.IsGemTile(tile))
                {
                    grid.GetTileValue(tile).Get<GemComponent>().gem.Deselect();
                }
            }
        }
    }
}
