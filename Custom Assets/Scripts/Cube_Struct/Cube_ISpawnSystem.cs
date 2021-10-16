using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

public struct Cube_ISpawnSystem : ISystemBase
{
    int maxCubesToSpawn;
    EntityQuery scjQuery;

    public void OnCreate(ref SystemState state)
    {
        maxCubesToSpawn = 0;
        scjQuery = state.GetEntityQuery(typeof(Cube_SpawnerData));
    }

    [BurstDiscard]
    public void OnUpdate(ref SystemState state)
    {
        if (maxCubesToSpawn < 1)
        {
            ComponentTypeHandle<Cube_SpawnerData> cubeSpawnerTypeHandle = state.GetComponentTypeHandle<Cube_SpawnerData>();
            SpawnCubeJob scj = new SpawnCubeJob
            {
                cubeSpawnerTypeHandle = cubeSpawnerTypeHandle,
                ecb = state.World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter()
            };
            state.Dependency = scj.ScheduleParallel(scjQuery, state.Dependency);
            state.World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>().AddJobHandleForProducer(state.Dependency);
            maxCubesToSpawn++;
        }
    }

    public void OnDestroy(ref SystemState state){}

    //Spawn
    [BurstCompile]
    struct SpawnCubeJob : IJobChunk
    {
        public ComponentTypeHandle<Cube_SpawnerData> cubeSpawnerTypeHandle;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Cube_SpawnerData> chunkSpawner = chunk.GetNativeArray(cubeSpawnerTypeHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                Entity e = ecb.Instantiate(i, chunkSpawner[i].prefab);
                ecb.AddComponent<Cube_ColorData>(i, e);
                ecb.AddComponent<Cube_MetallicData>(i, e);
                ecb.AddComponent<Cube_SmoothnessData>(i, e);
            }
        }
    }
}
