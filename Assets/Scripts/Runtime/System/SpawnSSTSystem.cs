using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{
    // That system spawn the Stimulus and also handle the steps when they are missed (stop signal and non stop signal)
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
                    sstIterator.responseTimeStimulus -= Time.DeltaTime; // Decrease the timing for the step

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

                                Debug.Log("Avoid a stop signal");
                                // Add the Ui Feedback OK
                                EntityManager.AddComponent(feedbackTarget, typeof(FeedbackOKTag));
                                session.nbrOfTrue++;
                                session.nbrOfTrueStop++;

                                // Set the succes on the actual step
                                sstIterator.actualStep.isSuccess = true;
                                // Set to the step the ssd used
                                sstIterator.actualStep.ssd = session.actualSSD;
                                // Adjust timing of SSD
                                float newSSD = StopSignalUtils.GetNextSSD(session, true);
                                // Get the next SSD for the session
                                session.actualSSD = newSSD;

                            } else { // Or a normal step FAILED

                                // Set the succes on the actual step
                                sstIterator.actualStep.isSuccess = false;
                                // Set to the step the ssd used
                                sstIterator.actualStep.ssd = session.actualSSD;

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
                        Debug.Log("TIME: " + sstIterator.actualStep.timeStartStimulus + "/ Actual Step: " + sstIterator.actualStep.index + " isStop:" + sstIterator.actualStep.isStopSignal + ", Food?" + sstIterator.actualStep.isFood + ", SSD: " + sstIterator.actualStep.ssd);// + ", IsFoodLeft?" + s.isFoodLeft);

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
