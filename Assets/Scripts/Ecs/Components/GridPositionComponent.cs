using UnityEngine;

namespace Scripts.Ecs.Components
{
    public struct GridPositionComponent
    {
        public Vector2Int gridPosition;

        public GridPositionComponent(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
        }
    }
}

