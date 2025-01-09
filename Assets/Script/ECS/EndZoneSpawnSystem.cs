using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EndZoneSpawnSystem : ISystem
{
    private bool hasSpawned;

    // Paramètres
    private const float radiusPillarsEnd = 15f;
    private const int nbPillarsEnd = 80;

    // On va ignorer un “faisceau” d’angle
    // Ex. deAngle = ±30° => ±(pie/6) en radians
    private const float skipAngle = math.PI / 8f;  // 30° en radians

    // Où placer cette zone (ex. fin du couloir)
    private const float endZoneZ = 200f; // Suppose que ton couloir s’arrête ~150
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndZonePrefabsData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        if (hasSpawned)
            return;
        hasSpawned = true;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        bool found = false;
        foreach (var endRef in SystemAPI.Query<RefRO<EndZonePrefabsData>>())
        {
            found = true;
            Entity pillarPrefab = endRef.ValueRO.pillarPrefab;
            Entity treePrefab = endRef.ValueRO.treePrefab;
            Entity treasurePrefab = endRef.ValueRO.treasurePrefab;
            Entity flowerPrefab = endRef.ValueRO.flowerPrefab;

            // 1) Placer des piliers / arbres sur un cercle, sauf dans l’angle skip
            for (int i = 0; i < nbPillarsEnd; i++)
            {
                float angleShift = math.PI / 2f; // 90 deg
                float angle = 2f * math.PI * i / nbPillarsEnd;


                // On veut skip ±30° => skipAngle = PI/6
                float skipAngle = math.PI / 6f;
                float angleN = math.atan2(math.sin(angle), math.cos(angle)); // => [-PI..+PI]
                if (math.abs(angleN + angleShift) < skipAngle)
                {
                    continue; // skip
                }


                float xPos = math.cos(angle) * radiusPillarsEnd;
                float zPos = math.sin(angle) * radiusPillarsEnd;

                // On ajoute un offset Z si on veut la zone en fin de couloir
                float3 pos = new float3(xPos, 0f, endZoneZ + zPos);

                // Choix aléatoire : arbre ou pilier
                bool usePillar = (UnityEngine.Random.value < 0.5f);
                Entity prefab = usePillar ? pillarPrefab : treePrefab;

                Entity e = ecb.Instantiate(prefab);

                float scale = UnityEngine.Random.Range(0.8f, 1.5f);
                quaternion rot = quaternion.Euler(0f, angle, 0f);

                ecb.SetComponent(e, new LocalTransform
                {
                    Position = pos,
                    Rotation = rot,
                    Scale = scale
                });
            }

            // 2) Placer le trésor au centre
            {
                Entity treasure = ecb.Instantiate(treasurePrefab);
                ecb.SetComponent(treasure, new LocalTransform
                {
                    Position = new float3(0f, 1f, endZoneZ),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }

            // 3) Placer éventuellement quelques fleurs random
            int flowerCount = 600;
            float flowerRadius = radiusPillarsEnd;
            for (int f = 0; f < flowerCount; f++)
            {
                float r = UnityEngine.Random.Range(1f, flowerRadius);
                float a = UnityEngine.Random.Range(0f, 2f * math.PI);

                // Idem, skip si on est dans l’angle d’entrée ?
                float angleN = math.atan2(math.sin(a), math.cos(a));
                if (math.abs(angleN) < skipAngle)
                    continue;

                float xPos = r * math.cos(a);
                float zPos = r * math.sin(a);

                Entity e = ecb.Instantiate(flowerPrefab);
                float scale = UnityEngine.Random.Range(0.5f, 1.2f);

                ecb.SetComponent(e, new LocalTransform
                {
                    Position = new float3(xPos, 0f, endZoneZ + zPos),
                    Rotation = quaternion.identity,
                    Scale = scale
                });
            }
        }

        if (!found)
            Debug.LogWarning("No EndZonePrefabsData found => cannot spawn end zone.");

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
