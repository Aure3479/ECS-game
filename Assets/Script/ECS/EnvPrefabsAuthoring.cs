using UnityEngine;
using Unity.Entities;

public class EnvPrefabsAuthoring : MonoBehaviour
{
    public GameObject pillarPrefab;
    public GameObject treePrefab;
}

// Baker
[BakingType]
public class EnvPrefabsBaker : Baker<EnvPrefabsAuthoring>
{
    public override void Bake(EnvPrefabsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        EnvPrefabsData data = new EnvPrefabsData
        {
            pillarPrefab = GetEntity(authoring.pillarPrefab, TransformUsageFlags.None),
            treePrefab = GetEntity(authoring.treePrefab, TransformUsageFlags.None)
        };
        AddComponent(entity, data);
    }
}


public struct EnvPrefabsData : IComponentData
{
    public Entity pillarPrefab;
    public Entity treePrefab;
}
