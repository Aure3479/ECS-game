using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct TrapSpawnSystem : ISystem
{
    private float timer;
    private const float spawnInterval = 3f;
    private const float couloirOffsetX = 6f; // ex. couloir = ±5, alors 6 c’est un peu hors couloir

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        timer += dt;

        if (timer < spawnInterval)
            return;

        // On reset le timer
        timer = 0f;

        // On crée un ecb
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // On récupère la liste de prefabs
        foreach (var trapRef in SystemAPI.Query<RefRO<TrapPrefabsData>>())
        {
            // Choix aléatoire du prefab
            int type = UnityEngine.Random.Range(0, 4); // Ex. 4 types : 0 = pivotHache, 1 = movingAxe, 2 = swordSpike, 3 = arrow
            Entity prefab;
            switch (type)
            {
                case 0: prefab = trapRef.ValueRO.pivotHache; break;
                case 1: prefab = trapRef.ValueRO.movingAxe; break;
                case 2: prefab = trapRef.ValueRO.swordSpike; break;
                default:
                    // Suppose qu'on a un arrowPrefab dans TrapPrefabsData (il faut l'ajouter).
                    // Sinon, vous pouvez stocker arrowPrefab dans un autre composant, ou
                    // avoir un champ "arrowPrefab" dans TrapPrefabsData.
                    prefab = Entity.Null;
                    break;
            }
            if (prefab == Entity.Null)
                return; // On arrête (vous pouvez log un warning)

            // On choisit d’où vient le piège : gauche ou droite ?
            bool fromLeft = (UnityEngine.Random.value < 0.5f);
            float xPos = fromLeft ? -couloirOffsetX : couloirOffsetX;

            // On peut choisir aussi une Y pos (ex. plus haut = Y=1f, plus bas = Y=0)
            float yPos = (UnityEngine.Random.value < 0.5f) ? 0f : 1f;

            // On instancie l'entité (via ecb)
            Entity newTrap = ecb.Instantiate(prefab);

            // On calcule la scale random
            float randomScale = 1f + UnityEngine.Random.Range(0f, 0.5f); // de 1 à 1.5
            // On calcule la rotation de base (si on veut pointer le piège vers le centre)
            quaternion rot = fromLeft ? quaternion.Euler(0, 0, 0) : quaternion.Euler(0, math.radians(180f), 0);

            // On place l'entité
            ecb.SetComponent(newTrap, new LocalTransform
            {
                Position = new float3(xPos, yPos, 0f),
                Rotation = rot,
                Scale = randomScale
            });

            // On lui ajoute un composant pour détruire s’il quitte ±couloirOffsetX * 1.2f (par ex.)
            ecb.AddComponent(newTrap, new DestroyWhenOutOfBoundData
            {
                limitX = couloirOffsetX * 1.2f,
                maxZ = 100f // par ex. si on veut détruire si le piège s'avance trop loin en Z
            });

            // On pourrait aussi augmenter la vitesse en fonction de la distance Z
            // si on veut un “plus c'est loin, plus c'est mortel” => Cf. plus bas
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
