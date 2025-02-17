using System.Collections.Generic;
using Arch.Core;
using Arch.System;
using Scripts.DataModels;
using Scripts.Ecs.Components;
using Scripts.Ecs.Systems;
using Scripts.GameView;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts.Ecs
{
    public partial class EcsEntry : MonoBehaviour
    {
        [SerializeField] private Gem gemPrefab;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private List<GemTypeSO> gemTypes;
        [SerializeField] private ScoreScreen scoreScreen;
        private Group<float> _systems;
        private MonoPool<Gem> _gemPool;

        private void Start() 
        {
            Application.targetFrameRate = 60;

            var world = World.Create();
            world.Create(new GridComponent(), new TilemapComponent(tilemap));

            GameObject gems = new GameObject("Gems");
            _gemPool = new MonoPool<Gem>(gemPrefab, gems.transform, tilemap.size.x * tilemap.size.y);

            PassiveItems.Items.Add(new SquarePeg());
            PassiveItems.Items.Add(new Clover());
            PassiveItems.Items.Add(new Tower());
            PassiveItems.Items.Add(new Holy());

            _systems = new Group<float>(
                "Match3",
                new InitializeGridSystem(world),
                new DrawDebugGridSystem(world),
                new SpawnGemsSystem(world, _gemPool.Pool, gemTypes),
                new InputSystem(world, inputReader),
                new TileSelectionSystem(world),
                new SwapTilesSystem(world),
                new MatchGemsSystem(world),
                new TriggerDispatchSystem(world),
                new TriggerGemsSystem(world),
                new ScoreSystem(world, scoreScreen),
                new ExplodeGemsSystem(world),
                new GemFallSystem(world),
                new UnblockGridSystem(world)
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
}
