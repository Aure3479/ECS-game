using UnityEngine;
using Unity.Entities;

// Ce script se met sur l�Objet parent (Arrow_1_White) dans le prefab
public class ArrowAuthoring : MonoBehaviour
{
    // Exemple de valeur, si tu veux faire bouger la fl�che
    public float moveSpeed = 5f;

    // Autres propri�t�s, ex. type de fl�che, d�g�ts, etc.

    public float type = 1f;
}

// POUR LA CONVERSION
[BakingType]
public class ArrowAuthoringBaker : Baker<ArrowAuthoring>
{
    public override void Bake(ArrowAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        // 1) On cr�e un composant (IComponentData) qu�on veut ajouter
        ArrowData arrowData = new ArrowData
        {
            MoveSpeed = authoring.moveSpeed,
            Type= authoring.type
        };

        // 2) On l�ajoute � l�entit� correspondante � la racine
        AddComponent(entity,arrowData);
    }
}

// Ce composant sera attach� � l�entit� �racine�
public struct ArrowData : IComponentData
{
    public float MoveSpeed;
    public float Type;
}

