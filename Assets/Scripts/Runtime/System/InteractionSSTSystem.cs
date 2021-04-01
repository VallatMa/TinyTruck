using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{

    public class InteractionSSTSystem : JobComponentSystem
    {
        private float spawnTimeStimulus;
        private Random randomGenerator;
        private EntityQuery shakeTarget;
        private EntityQuery feedbackTarget;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            shakeTarget = GetEntityQuery(ComponentType.ReadOnly(typeof(Shake)));
            feedbackTarget = GetEntityQuery(ComponentType.ReadOnly(typeof(FeedbackComponent)));
            RequireSingletonForUpdate<SSTIterator>();
            RequireSingletonForUpdate<ActiveInput>();
            spawnTimeStimulus = Const.SPAWN_DELAY;
            randomGenerator = new Random();
            randomGenerator.InitState();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            var sstIterator = GetSingleton<SSTIterator>();
            var ai = GetSingleton<ActiveInput>();
            SSTSession session = new SSTSession();

            try {
                session = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
            } catch { }

            if (sstIterator.isRunning) {

                float timeEndStimulus = (float)Time.ElapsedTime;

                int buttonId = ai.value.isButton;
                if (buttonId != 0) {
                    if (sstIterator.isWaitingForClickOrTimeOut) {

                        // Do the shake screen
                        EntityManager.AddComponent(shakeTarget, typeof(ActivatedTag));

                        sstIterator.actualStep.timeEndStimulus = timeEndStimulus;
                        sstIterator.actualStep.reactionTime = timeEndStimulus - sstIterator.actualStep.timeStartStimulus;
                        sstIterator.actualStep.isAnswered = true;

                        if (!sstIterator.actualStep.isStopSignal) {
                            bool isLeft = buttonId == 1 ? true : false;
                            // Check if it's right
                            if (sstIterator.actualStep.isFood) {
                                if ((session.isFoodLeft && isLeft) || (!session.isFoodLeft && !isLeft)) {
                                    // True
                                    sstIterator.actualStep.isSuccess = true;
                                }

                            } else {
                                if ((session.isFoodLeft && !isLeft) || (!session.isFoodLeft && isLeft)) {
                                    // True
                                    sstIterator.actualStep.isSuccess = true;
                                }
                            }

                            Debug.Log("STEP RIGHT?" + sstIterator.actualStep.isSuccess + " TIME: " + sstIterator.actualStep.reactionTime);

                            // Adjust session right and wrong
                            if (sstIterator.actualStep.isSuccess) {

                                // Show feedback
                                EntityManager.AddComponent(feedbackTarget, typeof(FeedbackOKTag));
                                session.nbrOfTrue++;
                            } else {

                                // Show feedback
                                EntityManager.AddComponent(feedbackTarget, typeof(FeedbackKOTag));
                                session.nbrOfFalse++;
                            }
                        } else {
                            Debug.Log("Missed the step");
                            sstIterator.actualStep.isSuccess = false;
                            // Show feedback
                            EntityManager.AddComponent(feedbackTarget, typeof(FeedbackKOTag));
                            session.nbrOfFalse++;

                            // Adjust timing of SSD
                            if (sstIterator.actualStep.isStopSignal) {

                                // Set to the step the ssd used
                                sstIterator.actualStep.ssd = session.actualSSD;
                                // Get the next SSD for the session
                                session.actualSSD = StopSignalUtils.GetNextSSD(session, false);
                            }
                        }
                    }

                    SetSingleton(new ActiveInput());
                    EntityManager.SetComponentData<SSTSession>(sstIterator.actualSessionEntity, session);
                    SetSingleton(sstIterator);
                }
            }

            return inputDeps;
        }

        /*public static void SSRTForActualStep(SSTSession s, StepStimulus step, DynamicBuffer<StepStimulus> steps)
        {
            float totalReactionTime = 0;
            float totalSSD = 0;

            for (int i = 0; i < s.activeStep; i++) {
                totalReactionTime += steps[i].reactionTime;
                totalSSD += steps[i].ssd;
            }

            float averageReactionTime = totalReactionTime / s.activeStep;
            step.averageReactionTime = averageReactionTime;
            float averageSSD = totalSSD / s.activeStep;

            step.ssrt = averageSSD - averageReactionTime;
            step.ssd = s.actualSSD;
        }*/

    }


}
