using UnityEngine;
using Unity.Entities;

// ----- DecorAuthoring.cs -----
public class DecorAuthoring : MonoBehaviour
{
    // Pas forcément de data pour l’instant,
    // mais on pourrait ajouter un scale, un style, etc.
}

// ----- DecorBaker.cs -----
[BakingType]
public class DecorBaker : Baker<DecorAuthoring>
{
    public override void Bake(DecorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        // On peut ajouter un simple tag
        AddComponent<DecorTag>(entity);
    }
}

public struct DecorTag : IComponentData { }
