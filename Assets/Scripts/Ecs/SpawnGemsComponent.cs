public struct SpawnGemsComponent
{
    public float delay;
    public float delayCounter;

    public SpawnGemsComponent(float delay)
    {
        this.delay = delay;
        delayCounter = 0f;
    }
}
