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
        // On s'assure de ne faire la génération qu'une seule fois (ou tant qu'on veut).
        if (hasSpawned) return;
        hasSpawned = true;

        // Récupérer l'entité qui possède PrefabReferenceData
        // (le "PrefabReferences" converti)
        // On suppose qu'il n'y en a qu'un dans la scène.
        foreach (var prefabRef in SystemAPI.Query<RefRO<PrefabReferenceData>>())
        {
            Entity arrowPrefab = prefabRef.ValueRO.arrowPrefab;

            // Vérifions qu'il n'est pas null
            if (arrowPrefab == Entity.Null)
            {
                Debug.LogWarning("[CorridorSpawnSystem] arrowPrefab est null !");
                return;
            }

            // Définissons quelques paramètres
            int nbLines = 5;           // 5 "lignes" de couloir
            float distanceBetweenLines = 10f;
            float labyrinthScale = 2f; // échelle globale

            // Boucle pour instancier des flèches le long de l'axe Z
            for (int i = 0; i < nbLines; i++)
            {
                // On calcule la position sur Z
                float3 position = new float3(0f, 0f, i * distanceBetweenLines);

                // Instancie la flèche
                Entity newArrow = state.EntityManager.Instantiate(arrowPrefab);

                // On modifie sa position, rotation, scale
                LocalTransform arrowTransform = LocalTransform.FromPositionRotationScale(
                    position,
                    quaternion.identity,
                    labyrinthScale // la flèche sera 2x plus grande que l'original
                );

                state.EntityManager.SetComponentData(newArrow, arrowTransform);
            }
        }
    }
}
