using System.Collections.Generic;
using Arch.Core;
using Arch.System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EcsEntry : MonoBehaviour
{
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private List<GemTypeSO> gemTypes;
    [SerializeField] private ParticleSystem explosionVfx;

    private Group<float> _systems;

    private void Start() 
    {
        var world = World.Create();
        world.Create(new GridComponent(), new TilemapComponent(tilemap));

        _systems = new Group<float>(
            "Match3",
            new InitializeGridSystem(world),
            new DrawDebugGridSystem(world),
            new SpawnGemsSystem(world, gemPrefab, gemTypes),
            new InputSystem(world, inputReader),
            new TileSelectionSystem(world),
            new SwapTilesSystem(world),
            new MatchGemsSystem(world),
            new ExplodeGemsSystem(world, explosionVfx)
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
