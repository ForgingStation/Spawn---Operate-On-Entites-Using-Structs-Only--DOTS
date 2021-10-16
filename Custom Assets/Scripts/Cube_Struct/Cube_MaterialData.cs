using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public struct Cube_MaterialData : IComponentData
{

}

[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct Cube_ColorData : IComponentData
{
    public float4 value;
}

[MaterialProperty("_Metallic", MaterialPropertyFormat.Float)]
public struct Cube_MetallicData : IComponentData
{
    public float value;
}

[MaterialProperty("_Smoothness", MaterialPropertyFormat.Float)]
public struct Cube_SmoothnessData : IComponentData
{
    public float value;
}


