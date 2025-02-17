using Arch.Core;
using Arch.System;
using Scripts.Ecs.Components;
using TMPro;
using UnityEngine;

namespace Scripts.Ecs.Systems
{
    public class DrawDebugGridSystem : BaseSystem<World, float>
    {
        private QueryDescription _desc = new QueryDescription().WithAll<GridComponent>();
        public DrawDebugGridSystem(World world) : base(world) { }

        public override void Initialize()
        {
            base.Initialize();
            // Run query, can also run multiple queries inside the update method
            World.Query(in _desc, (ref GridComponent grid) =>
            {
                DrawDebugLines(ref grid);
            });
        }

        private void DrawDebugLines(ref GridComponent grid)
        {
            const float duration = 100f;
            GameObject parent = new GameObject("Debugging");

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    CreateWorldText(parent, x + "," + y, grid.coordinateConverter.GridToWorldCenter(x, y, grid.cellSize, grid.origin), grid.coordinateConverter.Forward);
                    Debug.DrawLine(grid.coordinateConverter.GridToWorld(x, y, grid.cellSize, grid.origin), grid.coordinateConverter.GridToWorld(x, y + 1, grid.cellSize, grid.origin), Color.white, duration);
                    Debug.DrawLine(grid.coordinateConverter.GridToWorld(x, y, grid.cellSize, grid.origin), grid.coordinateConverter.GridToWorld(x + 1, y, grid.cellSize, grid.origin), Color.white, duration);
                }
            }

            Debug.DrawLine(grid.coordinateConverter.GridToWorld(0, grid.height, grid.cellSize, grid.origin), grid.coordinateConverter.GridToWorld(grid.width, grid.height, grid.cellSize, grid.origin), Color.white, duration);
            Debug.DrawLine(grid.coordinateConverter.GridToWorld(grid.width, 0, grid.cellSize, grid.origin), grid.coordinateConverter.GridToWorld(grid.width, grid.height, grid.cellSize, grid.origin), Color.white, duration);
        }

        private TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 direction,
        int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center, int sortingOrder = 0)
        {
            GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.position = position;
            gameObject.transform.forward = direction;

            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color == default ? Color.white : color;
            textMeshPro.alignment = textAnchor;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMeshPro;
        }
    }
}
