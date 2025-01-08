using UnityEngine;
using Unity.Entities;

// Ce script (MonoBehaviour) va stocker la liste de GameObjects prefab
// que vous souhaitez convertir en entités (Arrow, Hache, Arbre...).
public class PrefabReferenceAuthoring : MonoBehaviour
{
    // Ex. on veut juste la flèche pour commencer
    public GameObject arrowPrefab;

    // Vous pourrez en ajouter plus tard, par ex.:
    // public GameObject axePrefab;
    // public GameObject treePrefab;
    // etc.
}

[BakingType]
public class PrefabReferenceBaker : Baker<PrefabReferenceAuthoring>
{
    public override void Bake(PrefabReferenceAuthoring authoring)
    {
        // On crée un composant ECS qui va contenir les "Entity" correspondant
        // au prefab flèche, etc.
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        PrefabReferenceData data = new PrefabReferenceData();

        // On convertit le GameObject arrowPrefab en entité
        if (authoring.arrowPrefab != null)
        {
            Entity arrowEntityPrefab = GetEntity(authoring.arrowPrefab, TransformUsageFlags.None);
            data.arrowPrefab = arrowEntityPrefab;
        }

        AddComponent(entity, data);
    }
}

// Ce composant stocke les "Entity" prefabs pour différents objets
public struct PrefabReferenceData : IComponentData
{
    public Entity arrowPrefab;
    // public Entity axePrefab;
    // public Entity treePrefab;
}
