using UnityEngine;
using Unity.Entities;

public class SwordSpikeAuthoring : MonoBehaviour
{
    [Header("Spike settings")]
    public float cycleTime = 2f; // durée du cycle montée/descente
    public float amplitude = 2f; // hauteur max
}

[BakingType]
public class SwordSpikeBaker : Baker<SwordSpikeAuthoring>
{
    public override void Bake(SwordSpikeAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        SwordSpikeData data = new SwordSpikeData
        {
            CycleTime = authoring.cycleTime,
            Amplitude = authoring.amplitude
        };
        AddComponent(entity, data);
    }
}

public struct SwordSpikeData : IComponentData
{
    public float CycleTime;
    public float Amplitude;
    // On peut rajouter un timer interne dans un System
}
