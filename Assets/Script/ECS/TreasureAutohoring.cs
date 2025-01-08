using UnityEngine;
using Unity.Entities;

// ----- DecorAuthoring.cs -----
public class TreasureAuthoring : MonoBehaviour
{
    // Pas forcément de data pour l’instant,
    // mais on pourrait ajouter un scale, un style, etc.
}

// ----- DecorBaker.cs -----
[BakingType]
public class TreasureBaker : Baker<TreasureAuthoring>
{
    public override void Bake(TreasureAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        // On peut ajouter un simple tag
        AddComponent<TreasureTag>(entity);
    }
}

public struct TreasureTag : IComponentData { }
