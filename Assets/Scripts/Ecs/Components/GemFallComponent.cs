namespace Scripts.Ecs.Components
{
    public struct GemFallComponent
    {
        public float delay;
        public float delayCounter;

        public GemFallComponent(float delay)
        {
            this.delay = delay;
            delayCounter = 0f;
        }
    }
}

