using Unity.Entities;

[GenerateAuthoringComponent]
public struct Cube_SpawnerData : IComponentData
{
    public Entity prefab;
}
