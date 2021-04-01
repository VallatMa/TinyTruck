using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Tiny.Audio;

[AlwaysSynchronizeSystem]
public class SoundManagerSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SoundManager>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var soundManager = GetSingleton<SoundManager>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        TryPlaySounds(ecb, soundManager);

        ecb.Playback(EntityManager);
        ecb.Dispose();
        return default;
    }


    private void TryPlaySounds(EntityCommandBuffer ecb, SoundManager soundManager)
    {
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, in SoundRequest soundRequest) =>
            {
                if (soundManager.IsSoundEnabled) PlaySound(ecb, soundManager, soundRequest.Value);
                ecb.DestroyEntity(entity);
            }).Run();
    }

    private void PlaySound(EntityCommandBuffer ecb, SoundManager soundManager, SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.StopSignal:
                AudioSource audio = EntityManager.GetComponentData<AudioSource>(soundManager.StopSignal);
                ecb.AddComponent<AudioSourceStart>(soundManager.StopSignal);
                break;
        }
    }
    
}