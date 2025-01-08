using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MovingAxeAuthoring : MonoBehaviour
{
    [Header("Movement & Rotation")]
    public float moveSpeed = 5f;   // avance sur un axe
    public float spinSpeed = 180f; // rotation sur elle-même (deg/s)
}

[BakingType]
public class MovingAxeBaker : Baker<MovingAxeAuthoring>
{
    public override void Bake(MovingAxeAuthoring authoring)
    {
        // Récupérer l'Entity actuelle
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        // Créer les données du composant
        MovingAxeData data = new MovingAxeData
        {
            MoveSpeed = authoring.moveSpeed,
            SpinSpeed = math.radians(authoring.spinSpeed)
        };

        // Ajouter le composant à l'entité
        AddComponent(entity, data);
    }
}

public struct MovingAxeData : IComponentData
{
    public float MoveSpeed;
    public float SpinSpeed;
}

public struct LeftRightTrapData : IComponentData
{
    public float baseSpeed;     // vitesse de base
    public float direction;     // +1 = va vers la droite, -1 = va vers la gauche
    public float dangerFactor;  // on multiplie baseSpeed par (1 + dangerFactor)
}

