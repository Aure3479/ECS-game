using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct TreasureCollisionSystem : ISystem
{
    private ComponentLookup<PlayerTag> _playerLookup;
    private ComponentLookup<TreasureTag> _treasureLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _playerLookup = state.GetComponentLookup<PlayerTag>(true);
        _treasureLookup = state.GetComponentLookup<TreasureTag>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Update the lookups
        _playerLookup.Update(ref state);
        _treasureLookup.Update(ref state);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        // Make sure we have a GameState entity
        Entity gameState;
        bool hasGameState = SystemAPI.TryGetSingletonEntity<GameStateSingleton>(out gameState);
        if (!hasGameState)
        {
            gameState = ecb.CreateEntity();
            ecb.AddComponent(gameState, new GameStateSingleton());
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        // second ecb
        var ecb2 = new EntityCommandBuffer(Allocator.TempJob);

        var job = new TreasureCollisionJob
        {
            playerLookup = _playerLookup,
            treasureLookup = _treasureLookup,
            ecb = ecb2,
            gameState = gameState
        };

        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = job.Schedule(sim, state.Dependency);
        state.Dependency.Complete();

        ecb2.Playback(state.EntityManager);
        ecb2.Dispose();
    }

    [BurstCompile]
    struct TreasureCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<PlayerTag> playerLookup;
        [ReadOnly] public ComponentLookup<TreasureTag> treasureLookup;

        public EntityCommandBuffer ecb;
        public Entity gameState;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            bool aIsPlayer = playerLookup.HasComponent(entityA);
            bool bIsPlayer = playerLookup.HasComponent(entityB);

            bool aIsTreasure = treasureLookup.HasComponent(entityA);
            bool bIsTreasure = treasureLookup.HasComponent(entityB);

            if ((aIsPlayer && bIsTreasure) || (bIsPlayer && aIsTreasure))
            {
                // => collision Joueur - Trésor => Victory
                ecb.AddComponent<VictoryTag>(gameState);
            }
        }
    }
}
