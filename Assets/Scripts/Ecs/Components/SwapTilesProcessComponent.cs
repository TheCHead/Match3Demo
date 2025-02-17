using DG.Tweening;

namespace Scripts.Ecs.Components
{
    public struct SwapTilesProcessComponent
    {
        public Sequence sequence;

        public SwapTilesProcessComponent(Sequence sequence)
        {
            this.sequence = sequence;
        }
    }
}

