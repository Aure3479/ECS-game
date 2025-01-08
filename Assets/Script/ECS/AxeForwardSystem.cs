using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AxeForwardSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (axeData, transform)
                 in SystemAPI.Query<RefRO<MovingAxeData>, RefRW<LocalTransform>>())
        {
            float moveSpeed = axeData.ValueRO.MoveSpeed;
            float spinSpeed = axeData.ValueRO.SpinSpeed;

            // Avance sur Z
            float3 pos = transform.ValueRO.Position;
            pos.z += moveSpeed * dt;

            // Tourne autour de Y (autour de son axe vertical)
            quaternion rot = transform.ValueRO.Rotation;
            quaternion deltaRot = quaternion.AxisAngle(new float3(0, 0, 1), spinSpeed * dt);
            rot = math.mul(rot, deltaRot);

            // On met à jour
            transform.ValueRW.Position = pos;
            transform.ValueRW.Rotation = rot;
        }
    }
}
