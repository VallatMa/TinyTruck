using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{
    // That System is looping throught the sessions and also setting the next step in the Iterator
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
                SSTSession actualSession = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                actualSession.isSessionActive = true;

                if (sstIterator.actualStep.isAnswered) {

                    DynamicBuffer<StepStimulus> steps = EntityManager.GetBuffer<StepStimulus>(sstIterator.actualSessionEntity);

                    steps[actualSession.activeStep] = sstIterator.actualStep; // Set the actual step back into the list
                    actualSession.activeStep++; // increase the index of the step

                    if (actualSession.activeStep < actualSession.nbrOfStep) { // Setting the next step in the iterator if more step are to come
                        sstIterator.actualStep = steps[actualSession.activeStep]; // Set the next step
                        sstIterator.actualStep.isAnswered = false; // Set the need to answer the next step
                        sstIterator.actualStep.ssd = actualSession.actualSSD; // Set the actual SSD in the step
                        EntityManager.SetComponentData<SSTSession>(sstIterator.actualSessionEntity, actualSession); // Set the session back
                        Debug.Log("Changing step: " + actualSession.activeStep + "/" + actualSession.nbrOfStep);
                        SetSingleton(sstIterator); // Set the iterator

                    } else { // Otherwise, if no more steps, change to the next session

                        string sessionName = "";

                        // Show instruction between all the sessions
                        actualSession.isSessionActive = false;
                        actualSession.isSessionDone = true;

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

                        // Save the Session
                        string json = ToStringUtils.FormatSession(actualSession, sessionName, steps, sstHolder.nbrSessionDone);
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

            return inputDeps;
        }
    }


}
