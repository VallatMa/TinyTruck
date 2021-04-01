using Tiny2D;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SetupGameSystem : ComponentSystem
    {
        private Unity.Mathematics.Random randomGenerator;

        private int nbrOfStopSignal;
        private NativeList<int> nbrFood;
        private NativeList<int> nbrNonFood;

        protected override void OnStartRunning()
        {
            // Create the singeltons
            EntityManager.CreateEntity(typeof(ActiveInput)); // Singelton
            EntityManager.CreateEntity(typeof(SSTHolder)); // Singelton
            EntityManager.CreateEntity(typeof(SSTIterator)); // Singelton

            uint seedTime = (uint) (DateTime.Now.Hour * 100000 + DateTime.Now.Minute * 1000 + DateTime.Now.Second * 10 + DateTime.Now.Millisecond);
            Debug.Log("Seed time:" + seedTime);
            randomGenerator = new Unity.Mathematics.Random(seedTime);

            RequireSingletonForUpdate<SSTHolder>();
            RequireSingletonForUpdate<SSTIterator>();
            var sstHolder = new SSTHolder();
            var sstIterator = new SSTIterator();
            SetSingleton(sstHolder);
            SetSingleton(sstIterator);

            // Randomize the side of the food
            bool isFoodLeft = randomGenerator.NextBool();

            SSTSession stepsBlock1 = this.CreateBlankSession(isFoodLeft, false);
            SSTSession stepsBlock2 = this.CreateBlankSession(!isFoodLeft, false);
            SSTSession stepsTraining1 = this.CreateBlankSession(isFoodLeft, true);
            SSTSession stepsTraining2 = this.CreateBlankSession(!isFoodLeft, true);

            Entity entitySB1 = EntityManager.CreateEntity(typeof(SSTSession));
            Entity entitySB2 = EntityManager.CreateEntity(typeof(SSTSession));
            Entity entityST1 = EntityManager.CreateEntity(typeof(SSTSession));
            Entity entityST2 = EntityManager.CreateEntity(typeof(SSTSession));

            stepsTraining1.isSessionActive = true;
            EntityManager.SetComponentData(entitySB1, stepsBlock1);
            EntityManager.SetComponentData(entitySB2, stepsBlock2);
            EntityManager.SetComponentData(entityST1, stepsTraining1);
            EntityManager.SetComponentData(entityST2, stepsTraining2);

            EntityManager.AddBuffer<StepStimulus>(entitySB1);
            EntityManager.AddBuffer<StepStimulus>(entitySB2);
            EntityManager.AddBuffer<StepStimulus>(entityST1);
            EntityManager.AddBuffer<StepStimulus>(entityST2);

            DynamicBuffer<StepStimulus> bufSB1 = EntityManager.GetBuffer<StepStimulus>(entitySB1);
            DynamicBuffer<StepStimulus> bufSB2 = EntityManager.GetBuffer<StepStimulus>(entitySB2);
            DynamicBuffer<StepStimulus> bufST1 = EntityManager.GetBuffer<StepStimulus>(entityST1);
            DynamicBuffer<StepStimulus> bufST2 = EntityManager.GetBuffer<StepStimulus>(entityST2);

            this.PopulateSession(false, stepsBlock1, bufSB1);
            this.PopulateSession(false, stepsBlock2, bufSB2);
            this.PopulateSession(true, stepsTraining1, bufST1);
            this.PopulateSession(true, stepsTraining2, bufST2);

            sstHolder.nbrSessionDone = 0;
            sstHolder.stepsBlock1 = entitySB1;
            sstHolder.stepsBlock2 = entitySB2;
            sstHolder.stepsTraining1 = entityST1;
            sstHolder.stepsTraining2 = entityST2;

            sstIterator.isRunning = false;
            sstIterator.isWaitingForClickOrTimeOut = false;
            sstIterator.actualSessionEntity = entityST1; // Set the first training to be the first session
            sstIterator.actualStep = bufST1[0];
            sstIterator.spawnTimeStimulus = Const.SPAWN_DELAY;

            SetSingleton(sstHolder);
            SetSingleton(sstIterator);

        }

        // Create an empty session
        private SSTSession CreateBlankSession(bool isFoodLeft, bool isTraining)
        {
            return new SSTSession() {
                activeStep = 0,
                isSessionActive = false,
                nbrOfFalse = 0,
                nbrOfTrue = 0,
                nbrOfStep = isTraining ? Const.NBR_STIMULUS_TRAINING : Const.NBR_STIMULUS_BLOCK,
                isFoodLeft = isFoodLeft,
                actualSSD = Const.BASE_SSD
            };
        }

        // Initialize a session
        protected void PopulateSession(bool isTraining, SSTSession session, DynamicBuffer<StepStimulus> buf)
        {
            /*.nbrFood = new List<int>();
            this.nbrNonFood = new List<int>();*/
            this.nbrFood = new NativeList<int>(Const.NBR_IMAGE, Allocator.TempJob);
            this.nbrNonFood = new NativeList<int>(Const.NBR_IMAGE, Allocator.TempJob);


            StepStimulus[] steps; //= new List<StepStimulus>();

            session.activeStep = 0;

            if (isTraining) {
                session.nbrOfStep = Const.NBR_STIMULUS_TRAINING;
                steps = new StepStimulus[Const.NBR_STIMULUS_TRAINING];
            } else {
                session.nbrOfStep = Const.NBR_STIMULUS_BLOCK;
                steps = new StepStimulus[Const.NBR_STIMULUS_BLOCK];
            }

            this.nbrOfStopSignal = session.nbrOfStep / 4;

            // Fill the two list with int from 1 to 64
            for (int i = 1; i <= Const.NBR_IMAGE; i++) {
                nbrFood.Add(i);
                nbrNonFood.Add(i);
            }

            // Create the good number of step 
            for (int i = 0; i < session.nbrOfStep; i++) {
                steps[i] = CreateStep(i);
            }

            // Shuffle the list of steps
            this.Shuffle(steps);

            for (int i = 0; i < session.nbrOfStep; i++) {
                StepStimulus s = steps[i];
                s.index = i;
                buf.Add(s);
            }
            
            nbrFood.Dispose();
            nbrNonFood.Dispose();
        }

        // Create one step
        private StepStimulus CreateStep(int i)
        {
            bool isFood = i % 2 == 0 ? true: false ; // Half food, half non-food
            int newIndexImg = PickNumber(isFood); // Get the image for the food or non-food

            StepStimulus s = new StepStimulus {
                //s.index = i;
                indexImg = newIndexImg,
                isFood = isFood,
                isStopSignal = i < this.nbrOfStopSignal ? true : false, // Get the number of stop signal needed
                ssd = Const.BASE_SSD,
                timeEndStimulus = 0,
                reactionTime = 0
            };
            return s;
        }

        // Shuffle a list of steps
        private void Shuffle(StepStimulus[] list)
        {
            int n = list.Length;
            while (n > 1) {
                n--;
                int k = randomGenerator.NextInt(n + 1);
                StepStimulus value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Pick one number in the lists of food and non-food stimulus
        protected int PickNumber(bool isFood)
        {
            int nbr = -1;

            if (isFood) {
                int r = randomGenerator.NextInt(0, nbrFood.Length - 1);
                nbr = nbrFood[r];
                nbrFood.RemoveAt(r);
            } else {
                int r = randomGenerator.NextInt(0, nbrNonFood.Length - 1);
                nbr = nbrNonFood[r];
                nbrNonFood.RemoveAt(r);
            }

            return nbr;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //nbrFood.Dispose();
            //nbrNonFood.Dispose();
        }

    }
}
