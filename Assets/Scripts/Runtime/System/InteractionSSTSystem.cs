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

                int buttonId = ai.value.isButton;

                if (buttonId != 0) { // if a click append

                    // Do the shake screen
                    EntityManager.AddComponent(shakeTarget, typeof(ActivatedTag));

                    // If the game was awaiting the click, do more than just shaking the screen
                    if (sstIterator.isWaitingForClickOrTimeOut) {

                        // Get the actual time of the end of the stimulus when the click append and store some value in the actual Step in the iterator
                        float timeEndStimulus = (float)Time.ElapsedTime; 
                        sstIterator.actualStep.timeEndStimulus = timeEndStimulus;
                        sstIterator.actualStep.reactionTime = timeEndStimulus - sstIterator.actualStep.timeStartStimulus;
                        sstIterator.actualStep.isAnswered = true;

                        // If the step is not a stop signal
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

                            // Set to the step the ssd used
                            sstIterator.actualStep.ssd = session.actualSSD;

                        } else { // If the step is a stop signal

                            Debug.Log("Clicked on a stop signal");
                            sstIterator.actualStep.isSuccess = false;
                            EntityManager.AddComponent(feedbackTarget, typeof(FeedbackKOTag));// Show feedback
                            session.nbrOfFalse++;
                            session.nbrOfFalseStop++;

                            // Set to the step the ssd used
                            sstIterator.actualStep.ssd = session.actualSSD;
                            // Adjust timing of SSD
                            float newSSD = StopSignalUtils.GetNextSSD(session, false);
                            // Get the next SSD for the session
                            session.actualSSD = newSSD;
                        }
                    }

                    SetSingleton(new ActiveInput()); // Reset the click
                    EntityManager.SetComponentData<SSTSession>(sstIterator.actualSessionEntity, session);
                    SetSingleton(sstIterator);
                }
            }

            return inputDeps;
        }
    }


}
