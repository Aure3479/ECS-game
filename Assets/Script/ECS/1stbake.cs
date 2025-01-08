using UnityEngine;
using Unity.Entities;

// Ce script se met sur l’Objet parent (Arrow_1_White) dans le prefab
public class ArrowAuthoring : MonoBehaviour
{
    // Exemple de valeur, si tu veux faire bouger la flèche
    public float moveSpeed = 5f;

    // Autres propriétés, ex. type de flèche, dégâts, etc.

    public float type = 1f;
}

// POUR LA CONVERSION
[BakingType]
public class ArrowAuthoringBaker : Baker<ArrowAuthoring>
{
    public override void Bake(ArrowAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        // 1) On crée un composant (IComponentData) qu’on veut ajouter
        ArrowData arrowData = new ArrowData
        {
            MoveSpeed = authoring.moveSpeed,
            Type= authoring.type
        };

        // 2) On l’ajoute à l’entité correspondante à la racine
        AddComponent(entity,arrowData);
    }
}

// Ce composant sera attaché à l’entité “racine”
public struct ArrowData : IComponentData
{
    public float MoveSpeed;
    public float Type;
}

