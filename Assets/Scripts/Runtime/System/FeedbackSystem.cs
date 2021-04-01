using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace Assets.Scripts.Runtime
{

    [AlwaysSynchronizeSystem]
    public class FeedbackSystem : JobComponentSystem
    {
        private Random randomGenerator;

        protected override void OnStartRunning()
        {
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities
                .WithAll<FeedbackOKTag>()
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Translation t, ref FeedbackComponent f) => {
                    if (f.isOK) {
                        //Debug.Log("Feedback ok");
                        if (f.Duration == f.DurationLeft) {
                            t.Value.z = 0f;
                        }

                        if (0 < f.DurationLeft) {
                            f.DurationLeft -= Time.DeltaTime;
                        } else {
                            t.Value.z = 1f;
                            f.DurationLeft = f.Duration;
                            ecb.RemoveComponent<FeedbackOKTag>(entity);
                        }
                    } else {
                        ecb.RemoveComponent<FeedbackOKTag>(entity);
                    }
                }).Run();

            Entities
                .WithAll<FeedbackKOTag>()
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Translation t, ref FeedbackComponent f) => {
                    if (!f.isOK) {
                        //Debug.Log("Feedback KO");
                        if (f.Duration == f.DurationLeft) {
                            t.Value.z = 0f;
                        }

                        if (0 < f.DurationLeft) {
                            f.DurationLeft -= Time.DeltaTime;
                        } else {
                            t.Value.z = 1f;
                            f.DurationLeft = f.Duration;
                            ecb.RemoveComponent<FeedbackKOTag>(entity);
                        }
                    } else {
                        ecb.RemoveComponent<FeedbackKOTag>(entity);
                    }
                }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
            return default;
        }
        
    }


}
