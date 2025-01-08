using UnityEngine;
using Unity.Entities;

public class TrapPrefabsAuthoring : MonoBehaviour
{
    public GameObject pivotHachePrefab;
    public GameObject movingAxePrefab;
    public GameObject swordSpikePrefab;
}

[BakingType]
public class TrapPrefabsBaker : Baker<TrapPrefabsAuthoring>
{
    public override void Bake(TrapPrefabsAuthoring authoring)
    {
    Debug.Log("Baking TrapPrefabs...");

        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        TrapPrefabsData data = new TrapPrefabsData
        {
            pivotHache = GetEntity(authoring.pivotHachePrefab, TransformUsageFlags.None),
            movingAxe = GetEntity(authoring.movingAxePrefab, TransformUsageFlags.None),
            swordSpike = GetEntity(authoring.swordSpikePrefab, TransformUsageFlags.None)
        };
        AddComponent(entity, data);
    }
}

public struct TrapPrefabsData : IComponentData
{
    public Entity pivotHache;
    public Entity movingAxe;
    public Entity swordSpike;
}

