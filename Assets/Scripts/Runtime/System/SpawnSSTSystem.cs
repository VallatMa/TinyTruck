using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{

    public class SpawnSSTSystem : JobComponentSystem
    {
        private Random randomGenerator;
        private EntityQuery feedbackTarget;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            feedbackTarget = GetEntityQuery(ComponentType.ReadOnly(typeof(FeedbackComponent)));

            RequireSingletonForUpdate<SSTHolder>();

            randomGenerator = new Random();
            randomGenerator.InitState();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sstIterator = GetSingleton<SSTIterator>();

            if (sstIterator.isRunning) {

                if (sstIterator.isWaitingForClickOrTimeOut) {
                    sstIterator.responseTimeStimulus -= Time.DeltaTime;
                    // If the step is answered or the timeout is done
                    if (sstIterator.responseTimeStimulus < 0 || sstIterator.actualStep.isAnswered) {

                        // If the user missed the step
                        if (sstIterator.actualStep.isAnswered == false) {

                            // Get the session
                            SSTSession session = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                            
                            sstIterator.actualStep.isTimeOut = true;
                            sstIterator.actualStep.isAnswered = true;

                            // If it's a stop signal SUCCESS
                            if (sstIterator.actualStep.isStopSignal) {
                                sstIterator.actualStep.isSuccess = true;

                                // Add the Ui Feedback OK
                                EntityManager.AddComponent(feedbackTarget, typeof(FeedbackOKTag));

                                session.actualSSD = StopSignalUtils.GetNextSSD(session, false);
                                session.nbrOfTrue++;
                            } else { // Or a normal step FAILED
                                sstIterator.actualStep.isSuccess = false;
                                // Add the Ui Feedback KO
                                EntityManager.AddComponent(feedbackTarget, typeof(FeedbackKOTag));
                                session.nbrOfFalse++;
                            }

                            EntityManager.SetComponentData<SSTSession>(sstIterator.actualSessionEntity, session);
                        }

                        // Setup the timing for next stimulus
                        sstIterator.isWaitingForClickOrTimeOut = false;
                        //float randomTimeToDelay = randomGenerator.NextFloat(Const.SPAWN_FORK) - Const.SPAWN_FORK / 2; // Add random time 
                        sstIterator.spawnTimeStimulus = Const.SPAWN_DELAY;// + randomTimeToDelay; // next spawning time
                    }
                } else {
                    sstIterator.spawnTimeStimulus -= Time.DeltaTime;
                    // Spawne the next step
                    if (sstIterator.spawnTimeStimulus < 0) {

                        //Spawn the stimulus
                        sstIterator.actualStep.timeStartStimulus = (float)Time.ElapsedTime;
                        Debug.Log("TIME: " + sstIterator.actualStep.timeStartStimulus + "/ Actual Step: " + sstIterator.actualStep.index + " isStop:" + sstIterator.actualStep.isStopSignal + ", Food?" + sstIterator.actualStep.isFood);// + ", IsFoodLeft?" + s.isFoodLeft);

                        // Set the timer for the response
                        sstIterator.responseTimeStimulus = Const.RESPONSE_DELAY;
                        sstIterator.isWaitingForClickOrTimeOut = true;
                    }
                }

                SetSingleton(sstIterator);
            }

            return inputDeps;
        }

    }


}
