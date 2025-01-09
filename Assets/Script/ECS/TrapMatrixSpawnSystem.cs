using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

// Suppose qu'on a un DestroyWhenOutOfBoundData semblable à ton code

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct TrapMatrixSpawnSystem : ISystem
{
    private bool hasSpawned;

    // Paramètres du “couloir” en Z
    private const int nbLines = 60;    // nombre de segments
    private const int nbCols = 3;     // gauche / centre / droite
    private const float spacingZ = 3f; // distance entre lignes
    private const float spacingX = 4f; // distance entre colonnes
    private const float startZ = 10f;// commence un peu plus loin

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TrapPrefabsData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        // on le fait qu'une fois
        if (hasSpawned) return;
        hasSpawned = true;

        // on prépare un ECB
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        bool found = false;

        // On parcourt l'entité qui a TrapPrefabsData
        foreach (var trapRef in SystemAPI.Query<RefRO<TrapPrefabsData>>())
        {
            found = true;

            // On génère la matrice
            for (int i = 0; i < nbLines; i++)
            {
                for (int j = 0; j < nbCols; j++)
                {
                    // Sélection random du type
                    int type = UnityEngine.Random.Range(0, 4); // 0..3
                    Entity prefab;
                    switch (type)
                    {
                        case 0: prefab = trapRef.ValueRO.pivotHache; break;
                        case 1: prefab = trapRef.ValueRO.movingAxe; break;
                        case 2: prefab = trapRef.ValueRO.swordSpike; break;
                        case 3:
                            // tu dois avoir un arrowPrefab dans TrapPrefabsData
                            // si ce n'est pas le cas, on met Entity.Null
                            prefab = Entity.Null;
                            break;
                        default:
                            prefab = Entity.Null;
                            break;
                    }
                    if (prefab == Entity.Null)
                        continue;

                    // calcul de la position
                    float zPos = startZ + i * spacingZ;
                    float xPos = (j - (nbCols - 1) * 0.5f) * spacingX;
                    // ex. j=0 => -4, j=1 => 0, j=2 => +4

                    float3 position = new float3(xPos, 0f, zPos);

                    // on instancie
                    Entity e = ecb.Instantiate(prefab);

                    // random scale
                    float rScale = 1f + UnityEngine.Random.Range(0f, 1.5f);

                    // Rotation de base
                    quaternion baseRot = quaternion.identity;

                    // On set la transform
                    ecb.SetComponent(e, new LocalTransform
                    {
                        Position = position,
                        Rotation = baseRot,
                        Scale = rScale
                    });

                    // on ajoute un composant pour détruire
                    ecb.AddComponent(e, new DestroyWhenOutOfBoundData
                    {
                        limitX = 10f, // couloir ±10
                        maxZ = 220f
                    });

                    // on configure en fonction du type
                    switch (type)
                    {
                        case 0: // pivotHache
                            {
                                // si la colonne j=0 ou j=nbCols-1, on suppose "InWall=true"
                                // on peut "patcher" PivotTrapData. 
                                // or on laisse tel quel (il sera pivot par défaut)
                                break;
                            }
                        case 1: // movingAxe => on lui ajoute un LeftRightTrapData
                            {
                                Debug.Log("moving axe chosen");
                                float dir = 0f;
                                bool isLeft = (j == 0);
                                bool isRight = (j == nbCols - 1);

                                if (isLeft) dir = +1f;
                                if (isRight) dir = -1f;
                                // si c'est le milieu, random
                                if (!isLeft && !isRight)
                                    dir = (UnityEngine.Random.value < 0.5f) ? +1f : -1f;

                                float baseSpeed = UnityEngine.Random.Range(1f, 3f);

                                // plus la ligne est grande, plus c'est mortel => multiplier la vitesse 
                                float dangerFactor = i * 0.1f;

                                // on suppose que MovingAxe est déjà un composant 
                                // si on veut rajouter LeftRightTrapData en plus
                                ecb.AddComponent(e, new LeftRightTrapData
                                {
                                    baseSpeed = baseSpeed,
                                    direction = dir,
                                    dangerFactor = dangerFactor
                                });
                                break;
                            }
                        case 2: // swordSpike => rien de spécial, 
                            {
                                // on pourrait randomiser amplitude, cycle, etc.
                                break;
                            }
                        case 3: // arrow => on peut faire un composant "LeftRightTrapData" 
                            {
                                // ...
                                break;
                            }
                    }
                }
            }
        }

        if (!found)
        {
            Debug.LogWarning("No TrapPrefabsData found => no traps spawned.");
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
