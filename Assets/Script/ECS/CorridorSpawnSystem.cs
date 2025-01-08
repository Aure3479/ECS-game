using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CorridorSpawnSystem : ISystem
{
    private bool hasSpawned;

    public void OnUpdate(ref SystemState state)
    {
        // On s'assure de ne faire la g�n�ration qu'une seule fois (ou tant qu'on veut).
        if (hasSpawned) return;
        hasSpawned = true;

        // R�cup�rer l'entit� qui poss�de PrefabReferenceData
        // (le "PrefabReferences" converti)
        // On suppose qu'il n'y en a qu'un dans la sc�ne.
        foreach (var prefabRef in SystemAPI.Query<RefRO<PrefabReferenceData>>())
        {
            Entity arrowPrefab = prefabRef.ValueRO.arrowPrefab;

            // V�rifions qu'il n'est pas null
            if (arrowPrefab == Entity.Null)
            {
                Debug.LogWarning("[CorridorSpawnSystem] arrowPrefab est null !");
                return;
            }

            // D�finissons quelques param�tres
            int nbLines = 5;           // 5 "lignes" de couloir
            float distanceBetweenLines = 10f;
            float labyrinthScale = 2f; // �chelle globale

            // Boucle pour instancier des fl�ches le long de l'axe Z
            for (int i = 0; i < nbLines; i++)
            {
                // On calcule la position sur Z
                float3 position = new float3(0f, 0f, i * distanceBetweenLines);

                // Instancie la fl�che
                Entity newArrow = state.EntityManager.Instantiate(arrowPrefab);

                // On modifie sa position, rotation, scale
                LocalTransform arrowTransform = LocalTransform.FromPositionRotationScale(
                    position,
                    quaternion.identity,
                    labyrinthScale // la fl�che sera 2x plus grande que l'original
                );

                state.EntityManager.SetComponentData(newArrow, arrowTransform);
            }
        }
    }
}
