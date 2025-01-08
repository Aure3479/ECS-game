using Unity.Entities;

// Quand le joueur meurt
public struct GameOverTag : IComponentData { }

// Quand le joueur gagne
public struct VictoryTag : IComponentData { }
