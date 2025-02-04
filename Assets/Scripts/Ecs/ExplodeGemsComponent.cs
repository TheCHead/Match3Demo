public struct ExplodeGemsComponent
{
    public MatchSet matchSet;
    public float delay;
    public float delayCounter;

    public ExplodeGemsComponent(MatchSet matchSet, float delay)
    {
        this.matchSet = matchSet;
        this.delay = delay;
        delayCounter = 0f;
    }
}
