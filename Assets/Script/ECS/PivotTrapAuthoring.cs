using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

// Ex: PivotTrapAuthoring.cs
public class PivotTrapAuthoring : MonoBehaviour
{
	[Header("Rotation around pivot")]
	public float rotationSpeed = 45f; // degres/seconde
									  // Indique si c’est dans le sol, au mur, au plafond :
	public bool inFloor = false;
	public bool inWall = false;
}

// PivotTrapBaker.cs
[BakingType]
public class PivotTrapBaker : Baker<PivotTrapAuthoring>
{
	public override void Bake(PivotTrapAuthoring authoring)
	{
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        PivotTrapData data = new PivotTrapData
		{
			RotationSpeed = math.radians(authoring.rotationSpeed),
			InFloor = authoring.inFloor,
			InWall = authoring.inWall
		};
		AddComponent(entity, data);
	}
}

public struct PivotTrapData : IComponentData
{
	public float RotationSpeed; // en radians/s
	public bool InFloor;
	public bool InWall;
}

