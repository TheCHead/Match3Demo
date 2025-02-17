using UnityEngine.Tilemaps;

namespace Scripts.Ecs.Components
{
    public struct TilemapComponent
    {
        public Tilemap tilemap;

        public TilemapComponent(Tilemap tilemap)
        {
            this.tilemap = tilemap;
        }
    }
}

