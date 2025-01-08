using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ArrowMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // On parcourt toutes les entités qui ont un ArrowData et un LocalTransform
        foreach (var (arrowData, transform) in
                 SystemAPI.Query<RefRO<ArrowData>, RefRW<LocalTransform>>())
        {
            float3 forward = new float3(0, 0, 1);
            // On bouge vers l’avant
            transform.ValueRW.Position += forward * arrowData.ValueRO.MoveSpeed * dt;
        }
    }
}
