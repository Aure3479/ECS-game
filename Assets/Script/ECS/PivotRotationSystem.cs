using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PivotRotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (pivotData, transform)
                 in SystemAPI.Query<RefRO<PivotTrapData>, RefRW<LocalTransform>>())
        {
            float rotSpeed = pivotData.ValueRO.RotationSpeed;
            bool floor = pivotData.ValueRO.InFloor;
            bool wall = pivotData.ValueRO.InWall;

            // On applique une rotation continue
            quaternion currentRot = transform.ValueRO.Rotation;

            // Par défaut, on tourne autour de l'axe Y (si l'arme est plantée au sol)
            // Si c'est dans le mur, on pourrait tourner autour de X ou Z.
            float3 axis = new float3(0, 0, 1);
            if (wall) axis = new float3(0, 0, 1);

            quaternion deltaRot = quaternion.AxisAngle(axis, rotSpeed * dt);
            transform.ValueRW.Rotation = math.mul(currentRot, deltaRot);
        }
    }
}
