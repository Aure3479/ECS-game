using UnityEngine;
using Unity.Entities;

// StartZonePrefabsAuthoring.cs
public class StartZonePrefabsAuthoring : MonoBehaviour
{
    public GameObject treePrefab;
    public GameObject pillarPrefab;
    public GameObject flowerPrefab;

    // etc. si besoin
}

// StartZonePrefabsBaker.cs
[BakingType]
public class StartZonePrefabsBaker : Baker<StartZonePrefabsAuthoring>
{
    public override void Bake(StartZonePrefabsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        StartZonePrefabsData data = new StartZonePrefabsData
        {
            treePrefab = GetEntity(authoring.treePrefab, TransformUsageFlags.None),
            pillarPrefab = GetEntity(authoring.pillarPrefab, TransformUsageFlags.None),
            flowerPrefab = GetEntity(authoring.flowerPrefab, TransformUsageFlags.None)
        };

        AddComponent(entity, data);
    }
}

public struct StartZonePrefabsData : IComponentData
{
    public Entity treePrefab;
    public Entity pillarPrefab;
    public Entity flowerPrefab;
}
