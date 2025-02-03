using System.Collections.Generic;
using UnityEngine;

public struct ExplodeGemsComponent
{
    public List<Vector2Int> gems;
    public float delay;
    public float delayCounter;

    public ExplodeGemsComponent(List<Vector2Int> gems, float delay)
    {
        this.gems = gems;
        this.delay = delay;
        delayCounter = 0f;
    }
}
