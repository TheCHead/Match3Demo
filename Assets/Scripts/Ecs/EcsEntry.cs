using Arch.Core;
using Arch.System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EcsEntry : MonoBehaviour
{
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private InputReader inputReader;
    private Group<float> _systems;

    private void Start() 
    {
        var world = World.Create();
        world.Create(new GridComponent(), new TilemapComponent(tilemap));

        _systems = new Group<float>(
            "Match3",
            new InitializeGridSystem(world),
            new DrawDebugGridSystem(world),
            new SpawnGemsSystem(world, gemPrefab),
            new InputSystem(world, inputReader)
        );

        _systems.Initialize(); 
    }

    private void Update()
    {
        _systems.BeforeUpdate(Time.deltaTime);
        _systems.Update(Time.deltaTime);
        _systems.AfterUpdate(Time.deltaTime);
    }

    private void OnDestroy() 
    {
         _systems.Dispose();  
    }
}

public class InputSystem : BaseSystem<World, float>
{
    private QueryDescription _desc = new QueryDescription().WithAll<GridComponent, TilemapComponent>();
    public InputSystem(World world, InputReader reader) : base(world) {
        _inputReader = reader;
        _inputReader.FireEvent += OnFireEvent;
    }
    private InputReader _inputReader;

    private void OnFireEvent()
    {
        World.Query(in _desc, (ref GridComponent grid) => {
            var gridPos = grid.coordinateConverter.WorldToGrid(Camera.main.ScreenToWorldPoint(_inputReader.Selected), grid.cellSize, grid.origin);

            if (grid.IsValidTile(gridPos.x, gridPos.y))
            {
                Debug.Log($"Clicked tile: {gridPos.x}-{gridPos.y}");
            }
        });  
    }

    public override void Dispose()
    {
        base.Dispose();
        _inputReader.FireEvent -= OnFireEvent;
    }
}
