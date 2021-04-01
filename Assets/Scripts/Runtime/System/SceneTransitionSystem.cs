using Tiny2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Runtime
{
    [AlwaysSynchronizeSystem]
    public class SceneTransitionSystem : JobComponentSystem
    {
        protected override void OnStartRunning()
        {
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities
                .WithAll<SceneTransitionOutTag>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Translation translation, ref SceneTransition st) => {

                    if (0 < st.durationLeft) {
                        st.durationLeft -= Time.DeltaTime;
                        float3 pos = math.lerp(st.botPosition, st.topPosition, (st.durationLeft/st.duration));
                        translation.Value = pos;
                    } else {
                        ecb.RemoveComponent<SceneTransitionInTag>(entity);
                        st.durationLeft = st.duration;
                        var activeInput = GetSingleton<ActiveInput>();
                        activeInput.value = new ClickOnButton() {
                            isButton = 5 // 5 is for Switching from GamePlay to something else
                        };
                        SetSingleton(activeInput);
                    }
                }).Run();

            Entities
                .WithAll<SceneTransitionInTag>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Translation translation, ref SceneTransition st) => {

                    if (0 < st.durationLeft) {
                        st.durationLeft -= Time.DeltaTime;
                        float3 pos = math.lerp(st.topPosition, st.botPosition, st.durationLeft);
                        translation.Value = pos;
                    } else {
                        ecb.RemoveComponent<SceneTransitionInTag>(entity);
                        st.durationLeft = st.duration;
                        var activeInput = GetSingleton<ActiveInput>();
                        activeInput.value = new ClickOnButton() {
                            isButton = 5 // 5 is for Switching from GamePlay to something else
                        };
                        SetSingleton(activeInput);
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
            return default;
        }
    }


}
