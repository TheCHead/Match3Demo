using UnityEngine;

namespace Scripts.Ecs.Components
{
    public struct SwapTilesComponent
    {
        public Vector2Int tileA;
        public Vector2Int tileB;

        public SwapTilesComponent(Vector2Int tileA, Vector2Int tileB)
        {
            this.tileA = tileA;
            this.tileB = tileB;
        }
    }
}
