using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct Cube_ISystem : ISystemBase
{
    EntityQuery rcjQuery;
    EntityQuery ccjQuery;

    public void OnCreate(ref SystemState state)
    {
        rcjQuery = state.GetEntityQuery(typeof(Rotation), ComponentType.ReadOnly<Cube_ComponentData>());
        ccjQuery = state.GetEntityQuery(typeof(Cube_ColorData), typeof(Cube_MetallicData), typeof(Cube_SmoothnessData));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ComponentTypeHandle<Cube_ComponentData> cubeTypeHandle = state.GetComponentTypeHandle<Cube_ComponentData>();
        ComponentTypeHandle<Rotation> rotationTypeHandle = state.GetComponentTypeHandle<Rotation>();
        RotateCubeJob rcj = new RotateCubeJob
        {
            cubeTypeHandle = cubeTypeHandle,
            rotationTypeHandle = rotationTypeHandle
        };
        state.Dependency = rcj.ScheduleParallel(rcjQuery, state.Dependency);

        ComponentTypeHandle<Cube_ColorData> colorTypeHandle = state.GetComponentTypeHandle<Cube_ColorData>();
        ComponentTypeHandle<Cube_MetallicData> metallicTypeHandle = state.GetComponentTypeHandle<Cube_MetallicData>();
        ComponentTypeHandle<Cube_SmoothnessData> smoothnessTypeHandle = state.GetComponentTypeHandle<Cube_SmoothnessData>();
        CubeMaterialJob cmj = new CubeMaterialJob
        {
            colorTypeHandle = colorTypeHandle,
            metallicTypeHandle = metallicTypeHandle,
            smoothnessTypeHandle = smoothnessTypeHandle,
            randomColorValue = new Random((uint)(state.Time.ElapsedTime + 1) * 100).NextFloat4(0, 1),
            randomSmoothenss = new Random((uint)(state.Time.ElapsedTime + 5) * 800).NextFloat(0, 1),
            randomMetallic = new Random((uint)(state.Time.ElapsedTime + 12) * 1500).NextFloat(0, 1),
        };
        state.Dependency = cmj.ScheduleParallel(ccjQuery, state.Dependency);
    }

    public void OnDestroy(ref SystemState state) { }

    //Rotate
    [BurstCompile]
    struct RotateCubeJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<Cube_ComponentData> cubeTypeHandle;
        public ComponentTypeHandle<Rotation> rotationTypeHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Cube_ComponentData> chunkCube = chunk.GetNativeArray(cubeTypeHandle);
            NativeArray<Rotation> chunkRotation = chunk.GetNativeArray(rotationTypeHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                chunkRotation[i] = new Rotation
                {
                    Value = math.mul(chunkRotation[i].Value, quaternion.RotateY(chunkCube[i].rotationSpeed))
                };
            }
        }
    }

    //change material propery
    [BurstCompile]
    struct CubeMaterialJob : IJobChunk
    {
        public ComponentTypeHandle<Cube_ColorData> colorTypeHandle;
        public ComponentTypeHandle<Cube_MetallicData> metallicTypeHandle;
        public ComponentTypeHandle<Cube_SmoothnessData> smoothnessTypeHandle;
        public float4 randomColorValue;
        public float randomSmoothenss;
        public float randomMetallic;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Cube_ColorData> chunkColor = chunk.GetNativeArray(colorTypeHandle);
            NativeArray<Cube_MetallicData> chunkMetallic = chunk.GetNativeArray(metallicTypeHandle);
            NativeArray<Cube_SmoothnessData> chunkSmoothness = chunk.GetNativeArray(smoothnessTypeHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                chunkColor[i] = new Cube_ColorData
                {
                    value = randomColorValue
                };
                chunkMetallic[i] = new Cube_MetallicData
                {
                    value = randomMetallic
                };
                chunkSmoothness[i] = new Cube_SmoothnessData
                {
                    value = randomSmoothenss
                };
            }
        }
    }
}

