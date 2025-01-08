using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

/// <summary>
/// A single ECS system that detects collisions for:
///    - (Player, Trap) => Add GameOverTag
///    - (Player, Treasure) => Add VictoryTag
/// using ICollisionEventsJob, with a safe ECB pattern in Entities 1.0.
///
/// It creates a "game state" entity in OnCreate, so no external creation is needed.
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct GameCollisionSystem : ISystem
{
    // ---------------------------------------------------
    // Private fields
    // ---------------------------------------------------
    private ComponentLookup<PlayerTag> _playerLookup;
    private ComponentLookup<TrapTag> _trapLookup;
    private ComponentLookup<TreasureTag> _treasureLookup;

    private Entity _gameStateEntity; // We'll put GameOverTag / VictoryTag on this.

    // ---------------------------------------------------
    // OnCreate: set up lookups + create a game-state entity
    // ---------------------------------------------------
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // 1) Create a single entity for "game state" (where we add GameOverTag / VictoryTag).
        _gameStateEntity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(_gameStateEntity, new GameStateSingleton());

        // 2) Build read-only lookups for PlayerTag, TrapTag, TreasureTag
        //    We'll update them each frame in OnUpdate.
        _playerLookup = state.GetComponentLookup<PlayerTag>(true);
        _trapLookup = state.GetComponentLookup<TrapTag>(true);
        _treasureLookup = state.GetComponentLookup<TreasureTag>(true);
    }

    // ---------------------------------------------------
    // OnUpdate: schedule the collision job, use ECB system
    // ---------------------------------------------------
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // (A) Update the lookups to match current structural changes
        _playerLookup.Update(ref state);
        _trapLookup.Update(ref state);
        _treasureLookup.Update(ref state);

        // (B) Retrieve the EndFixedStepSimulationEntityCommandBufferSystem "singleton"
        //     in Entities 1.0 via SystemAPI:
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

        // (C) Create an ECB from that singleton. We do NOT store the system as a field
        //     because it's managed. Instead, we only retrieve the singleton each frame.
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // (D) Build the collision job that checks for Player-Trap & Player-Treasure
        var collisionJob = new GameCollisionJob
        {
            playerLookup = _playerLookup,
            trapLookup = _trapLookup,
            treasureLookup = _treasureLookup,
            ecb = ecb.AsParallelWriter(),
            gameState = _gameStateEntity
        };

        // (E) Schedule via the physics SimulationSingleton
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = collisionJob.Schedule(sim, state.Dependency);


    }

    // ---------------------------------------------------
    // The collision job that checks trap + treasure
    // ---------------------------------------------------
    [BurstCompile]
    struct GameCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<PlayerTag> playerLookup;
        [ReadOnly] public ComponentLookup<TrapTag> trapLookup;
        [ReadOnly] public ComponentLookup<TreasureTag> treasureLookup;

        public EntityCommandBuffer.ParallelWriter ecb;
        public Entity gameState;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            bool aIsPlayer = playerLookup.HasComponent(entityA);
            bool bIsPlayer = playerLookup.HasComponent(entityB);

            bool aIsTrap = trapLookup.HasComponent(entityA);
            bool bIsTrap = trapLookup.HasComponent(entityB);

            bool aIsTreasure = treasureLookup.HasComponent(entityA);
            bool bIsTreasure = treasureLookup.HasComponent(entityB);

            // 1) If (Player, Trap) => Game Over
            if ((aIsPlayer && bIsTrap) || (bIsPlayer && aIsTrap))
            {
                UnityEngine.Debug.Log("Collision: Player + Trap => Game Over!");
                ecb.AddComponent<GameOverTag>(0, gameState);
            }

            // 2) If (Player, Treasure) => Victory
            if ((aIsPlayer && bIsTreasure) || (bIsPlayer && aIsTreasure))
            {
                UnityEngine.Debug.Log("Collision: Player + Trap => Win!");
                ecb.AddComponent<VictoryTag>(0, gameState);
            }
        }
    }
}
