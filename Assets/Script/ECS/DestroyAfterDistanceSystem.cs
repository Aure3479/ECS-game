using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public struct DestroyWhenOutOfBoundData : IComponentData
{
    public float limitX; // ex.  ± 10f
    public float maxZ;   // ex.  50f
}

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct DestroyAfterBoundSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // On va utiliser un ECB pour détruire
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bound, transform, entity)
                 in SystemAPI.Query<RefRO<DestroyWhenOutOfBoundData>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            float3 pos = transform.ValueRO.Position;
            if (pos.x > bound.ValueRO.limitX || pos.x < -bound.ValueRO.limitX || pos.z > bound.ValueRO.maxZ)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
