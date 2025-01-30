using UnityEngine.Tilemaps;

public struct TilemapComponent
{
    public Tilemap tilemap;

    public TilemapComponent(Tilemap tilemap)
    {
        this.tilemap = tilemap;
    }
}
