using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class TrapBaseAuthoring : MonoBehaviour
{
    // Pour ajuster la rotation de base
    public Vector3 baseRotationEuler = Vector3.zero;
    // Pour ajuster un scale de base
    public float baseScale = 1f;
}

[BakingType]
public class TrapBaseBaker : Baker<TrapBaseAuthoring>
{
    public override void Bake(TrapBaseAuthoring authoring)
    {
        // On récupère l'entité
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        // On convertit la rotation en radians
        float3 eulerRad = math.radians(authoring.baseRotationEuler);
        quaternion baseRot = quaternion.Euler(eulerRad);

        // On crée un composant qui stocke ces infos de “param par défaut”
        TrapBaseData data = new TrapBaseData
        {
            BaseRotation = baseRot,
            BaseScale = authoring.baseScale
        };

        AddComponent(entity, data);
    }
}

public struct TrapBaseData : IComponentData
{
    public quaternion BaseRotation;
    public float BaseScale;
}
public struct TrapTag : IComponentData { }

