using UnityEngine;
using Unity.Entities;


public class EndZonePrefabsAuthoring : MonoBehaviour
{
    public GameObject treePrefab;
    public GameObject pillarPrefab;
    public GameObject flowerPrefab;
    public GameObject treasurePrefab;

    // etc. si besoin
}

// StartZonePrefabsBaker.cs
[BakingType]
public class EndZonePrefabsBaker : Baker<EndZonePrefabsAuthoring>
{
    public override void Bake(EndZonePrefabsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        EndZonePrefabsData data = new EndZonePrefabsData
        {
            treePrefab = GetEntity(authoring.treePrefab, TransformUsageFlags.None),
            pillarPrefab = GetEntity(authoring.pillarPrefab, TransformUsageFlags.None),
            flowerPrefab = GetEntity(authoring.flowerPrefab, TransformUsageFlags.None),
            treasurePrefab = GetEntity(authoring.treasurePrefab, TransformUsageFlags.None)
        };

        AddComponent(entity, data);
    }
}

public struct EndZonePrefabsData : IComponentData
{
    public Entity treePrefab;
    public Entity pillarPrefab;
    public Entity flowerPrefab;
    public Entity treasurePrefab;
}
