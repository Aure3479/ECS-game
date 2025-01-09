using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using System;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct StartZoneSpawnSystem : ISystem
{
    private bool hasSpawned;

    // Paramètres : vous pouvez en faire des const ou variables
    private const float radiusPillars = 15f;   // cercle “bordure”
    private const float radiusFlowers = 12f;   // fleurs jusqu’à 15
    private const float radiusEmptyCenter = 3f;// zone vide au centre
    private const int nbPillars = 50;   // piliers sur le périmètre
    private const int nbFlowers = 200;   // quantités de fleurs
    private const float skipAngle = math.PI / 8f;  // 30° en radians
    private const float angleShift = math.PI / 2f; // 90° => on décale l'angle
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StartZonePrefabsData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (hasSpawned) return;
        hasSpawned = true;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        bool found = false;
        for (int j = 0; j < 2; j++)
        {
            foreach (var startRef in SystemAPI.Query<RefRO<StartZonePrefabsData>>())
            {
                found = true;

                Entity pillarPrefab = startRef.ValueRO.pillarPrefab;
                Entity treePrefab = startRef.ValueRO.treePrefab;
                Entity flowerPrefab = startRef.ValueRO.flowerPrefab;

                // 1) Placer nbPillars sur le cercle “radiusPillars”
                for (int i = 0; i < nbPillars; i++)
                {
                    float angleShift = math.PI / 2f; // 90 deg
                    float angle = 2f * math.PI * i / nbPillars;


                    // On veut skip ±30° => skipAngle = PI/6
                    float skipAngle = math.PI / 6f;
                    float angleN = math.atan2(math.sin(angle), math.cos(angle)); // => [-PI..+PI]
                    if (math.abs(angleN - angleShift) < skipAngle)
                    {
                        continue; // skip
                    }


                    float3 pos = new float3(
                        math.cos(angle) * (radiusPillars+ 2*j),
                        3f,
                        math.sin(angle) * (radiusPillars + 2*j)
                    );


                    // On choisit aléatoirement un “pilier” ou un “tree” ou un mix 
                    bool usePillar = (UnityEngine.Random.value < 0.5f);
                    Entity prefab = usePillar ? pillarPrefab : treePrefab;

                    Entity e = ecb.Instantiate(prefab);
                    float scale = UnityEngine.Random.Range(1.8f, 3.5f);

                    quaternion rot = quaternion.Euler(0f, angle, 0f);
                    // Juste pour orienter un peu

                    ecb.SetComponent(e, new LocalTransform
                    {
                        Position = pos,
                        Rotation = rot,
                        Scale = scale
                    });
                }

                // 2) Placer nbFlowers dans le cercle, hors center
                for (int i = 0; i < nbFlowers; i++)
                {
                    float r = UnityEngine.Random.Range(radiusEmptyCenter, radiusFlowers);
                    float angle = UnityEngine.Random.Range(0f, 2f * math.PI);
                    float3 pos = new float3(
                        r * math.cos(angle),
                        0f,
                        r * math.sin(angle)
                    );

                    Entity e = ecb.Instantiate(flowerPrefab);

                    float scale = UnityEngine.Random.Range(2.5f, 3f);

                    // On applique une petite rotation random
                    float randomAngleY = UnityEngine.Random.Range(0f, 360f) * math.PI / 180f;
                    quaternion rot = quaternion.Euler(0f, randomAngleY, 0f);

                    ecb.SetComponent(e, new LocalTransform
                    {
                        Position = pos,
                        Rotation = rot,
                        Scale = scale
                    });
                }
            }
        }
        if (!found)
            Debug.LogWarning("No StartZonePrefabsData found => cannot spawn start zone.");

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
