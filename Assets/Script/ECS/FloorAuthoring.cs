using UnityEngine;
using Unity.Entities;

public class FloorAuthoring : MonoBehaviour
{
    public Vector2 size = new Vector2(10, 50); // large x 10, long z 50
}

[BakingType]
public class FloorBaker : Baker<FloorAuthoring>
{
    public override void Bake(FloorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        // On peut stocker la taille
        AddComponent(entity, new FloorData
        {
            Width = authoring.size.x,
            Length = authoring.size.y
        });
    }
}

public struct FloorData : IComponentData
{
    public float Width;
    public float Length;
}
