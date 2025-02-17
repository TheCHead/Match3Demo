using Scripts.GameView;

namespace Scripts.Ecs.Components
{
    public struct GemComponent
    {
        public Gem gem;

        public GemComponent(Gem gem)
        {
            this.gem = gem;
        }
    }
}


