using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Tiny;
using Unity.Tiny.Audio;

namespace Assets.Scripts.Runtime
{

    public class StopSignalSystem : JobComponentSystem
    {
        private float spawnTimeStimulus;
        private int index;
        private bool needSound;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            RequireSingletonForUpdate<SSTIterator>();
            index = 0;
            needSound = true;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sstIterator = GetSingleton<SSTIterator>();

            if (sstIterator.isRunning) {

                if (index != sstIterator.actualStep.index) {
                    index = sstIterator.actualStep.index;
                    needSound = true;
                }


                if (sstIterator.isWaitingForClickOrTimeOut && sstIterator.actualStep.isStopSignal) {

                    SSTSession session = new SSTSession();

                    try {
                        session = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                    } catch { }


                    float stopTiming = sstIterator.actualStep.timeStartStimulus + session.actualSSD;

                    if (((float)Time.ElapsedTime >= stopTiming) && needSound) {
                        needSound = false;
                        // Show The stop signal
                        Debug.Log("STOP SIGNAL, Time: " + Time.ElapsedTime + " ssd time:" + session.actualSSD);

                        EntityManager.AddComponentData(EntityManager.CreateEntity(), new SoundRequest { Value = SoundType.StopSignal });
                    }
                }
            }

            return inputDeps;
        }

    }


}
