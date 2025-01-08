using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SwordSpikesSystem : ISystem
{
    private float cycleTimer; // ou alors on stocke un timer par entit� ?

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (spikeData, transform)
                 in SystemAPI.Query<RefRO<SwordSpikeData>, RefRW<LocalTransform>>())
        {
            float cycleTime = spikeData.ValueRO.CycleTime;
            float amplitude = spikeData.ValueRO.Amplitude;

            // Calcul d'une oscillation
            // ex. on peut utiliser sin() pour monter/descendre
            // Id�alement, on aurait un timer par entit�,
            // mais ici on fait un �global time� simplifi�.  
            // Pour un usage correct, on stocke un �phase� par entit�.  

            float globalTime = (float)SystemAPI.Time.ElapsedTime;
            float phase = (globalTime % cycleTime) / cycleTime; // fraction [0..1]

            // ex. on monte de 0 � amplitude, puis on redescend
            // On peut faire un cycle simple : t -> sin(t * 2pi)
            float angle = phase * (math.PI * 2f);
            float height = amplitude * 0.5f * (1f + math.sin(angle));

            float3 pos = transform.ValueRO.Position;
            pos.y = height; // on imagine que la base du spike est y=0

            transform.ValueRW.Position = pos;
        }
    }
}
