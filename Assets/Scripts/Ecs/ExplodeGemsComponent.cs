using System.Collections.Generic;
using UnityEngine;

public struct ExplodeGemsComponent
{
    public List<Vector2Int> gems;

    public ExplodeGemsComponent(List<Vector2Int> gems)
    {
        this.gems = gems;
    }
}
