using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnvironmentSideSpawnSystem : ISystem
{
    private bool hasSpawned;
    public void OnUpdate(ref SystemState state)
    {
        if (hasSpawned)
            return;
        hasSpawned = true;

        // On utilise un ECB
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // On parcourt nos EnvPrefabs
        foreach (var envRef in SystemAPI.Query<RefRO<EnvPrefabsData>>())
        {
            var pillar = envRef.ValueRO.pillarPrefab;
            var tree = envRef.ValueRO.treePrefab;

            if (pillar == Entity.Null || tree == Entity.Null)
            {
                UnityEngine.Debug.LogWarning("EnvPrefabs: pillar or tree is NULL!");
                continue;
            }

            // 20 segments
            int nbSegments = 20;
            float spacing = 10f;
            float offsetX = 5f;

            for (int i = 0; i < nbSegments; i++)
            {
                float zPos = i * spacing;

                // On crée, par exemple, 3 décors à gauche + 3 décors à droite, 
                // avec de légers offsets en X, Z, et un scale random.
                SpawnMultipleDecors(ref ecb, pillar, tree, -offsetX, zPos, 12);
                SpawnMultipleDecors(ref ecb, pillar, tree, +offsetX, zPos, 12);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void SpawnMultipleDecors(
        ref EntityCommandBuffer ecb,
        Entity pillar,
        Entity tree,
        float baseX,
        float baseZ,
        int count)
    {
        for (int k = 0; k < count; k++)
        {
            bool usePillar = (UnityEngine.Random.value < 0.5f);
            Entity prefab = usePillar ? pillar : tree;

            // On écarte un peu l’offset en X, 
            // et on rajoute un random local offset en Z 
            float localOffsetX = UnityEngine.Random.Range(-2f, 2f);
            float localOffsetZ = UnityEngine.Random.Range(-5f, 5f);

            float3 pos = new float3(baseX + localOffsetX, 0f, baseZ + localOffsetZ);

            float2 pos2D = new float2(baseX + localOffsetX, baseZ + localOffsetZ);
            float2 centerStart = new float2(0, 0);
            float radiusDepart = 15f; // ex. 
            if (math.distance(pos2D, centerStart) < radiusDepart)
            {
                // skip => n'instancie pas
                continue;
            }
            
            else
            {
                // random scale
                float randomScale = UnityEngine.Random.Range(1f, 4f);

                Entity e = ecb.Instantiate(prefab);

                ecb.SetComponent(e, new LocalTransform
                {
                    Position = pos,
                    Rotation = quaternion.identity,
                    Scale = randomScale
                });
            }
        }
    }
}

