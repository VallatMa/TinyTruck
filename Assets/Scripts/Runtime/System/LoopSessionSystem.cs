using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{

    public class LoopSessionSystem : JobComponentSystem
    {
        private Random randomGenerator;
        private EntityQuery changeSceneTarget;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            RequireSingletonForUpdate<SSTHolder>();
            RequireSingletonForUpdate<SSTIterator>();

            randomGenerator = new Random();
            randomGenerator.InitState();

            changeSceneTarget = GetEntityQuery(ComponentType.ReadOnly(typeof(SceneTransition)));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sstIterator = GetSingleton<SSTIterator>();
            var sstHolder = GetSingleton<SSTHolder>();

            if (sstIterator.isRunning) {

                // Get the actual step from the session
                SSTSession s = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                s.isSessionActive = true;

                if (s.isSessionActive) {

                    if (sstIterator.actualStep.isAnswered) {

                        DynamicBuffer<StepStimulus> steps = EntityManager.GetBuffer<StepStimulus>(sstIterator.actualSessionEntity);

                        steps[s.activeStep] = sstIterator.actualStep;// Set the actual step
                        s.activeStep++;

                        if (s.activeStep < s.nbrOfStep) {
                            sstIterator.actualStep = steps[s.activeStep]; // Set the next step
                            sstIterator.actualStep.isAnswered = false; // Set the need to answer the next step
                            EntityManager.SetComponentData<SSTSession>(sstIterator.actualSessionEntity, s);
                            Debug.Log("Changing step: " + s.activeStep + "/" + s.nbrOfStep);
                            SetSingleton(sstIterator);

                        } else {

                            // Change the session
                            string sessionName = "";

                            // Show instruction between all the sessions
                            s.isSessionActive = false;
                            s.isSessionDone = true;

                            DynamicBuffer<StepStimulus> stepsNew = new DynamicBuffer<StepStimulus>();
                            if (sstHolder.nbrSessionDone == 0) {
                                sessionName = "Training 1";
                                sstIterator.actualSessionEntity = sstHolder.stepsBlock1;
                                stepsNew = EntityManager.GetBuffer<StepStimulus>(sstHolder.stepsBlock1);
                            }

                            if (sstHolder.nbrSessionDone == 1) {
                                sessionName = "Block 1";
                                sstIterator.actualSessionEntity = sstHolder.stepsTraining2;
                                stepsNew = EntityManager.GetBuffer<StepStimulus>(sstHolder.stepsTraining2);
                            }

                            if (sstHolder.nbrSessionDone == 2) {
                                sessionName = "Training 2";
                                sstIterator.actualSessionEntity = sstHolder.stepsBlock2;
                                stepsNew = EntityManager.GetBuffer<StepStimulus>(sstHolder.stepsBlock2);
                            }
                            
                            if (sstHolder.nbrSessionDone == 3) {
                                sessionName = "Block 2";
                                Debug.Log("Last session Done");
                            }

                            Debug.Log("Session, " + sessionName + " is finished, Food is left?" + s.isFoodLeft + " Nbr of session done: " + sstHolder.nbrSessionDone);

                            // Save the Session
                            string json = ToStringUtils.FormatSession(s, sessionName, steps, sstHolder.nbrSessionDone);
                            FileUtils.SaveInput(json);

                            // Increment nbr of session done
                            sstHolder.nbrSessionDone++;

                            // Set the iterator with the new step
                            sstIterator.isRunning = false;
                            if (sstHolder.nbrSessionDone < 4) {
                                sstIterator.actualStep = stepsNew[0];// Set the actual step
                            }

                            SetSingleton(sstHolder);
                            SetSingleton(sstIterator);

                            EntityManager.AddComponent(changeSceneTarget, typeof(SceneTransitionOutTag));
                        }
                    }
                }
            }


            return inputDeps;
        }
    }


}
