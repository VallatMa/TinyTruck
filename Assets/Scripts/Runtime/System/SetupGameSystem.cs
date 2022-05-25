using Tiny2D;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{
    // Setup Game System initialise the lists of stimulus used during the whole game
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SetupGameSystem : ComponentSystem
    {
        private Unity.Mathematics.Random randomGenerator;

        private sbyte nbrOfStopSignalTraining;
        private sbyte nbrOfStopSignalBlock;
        private NativeList<sbyte> nbrFood;
        private NativeList<sbyte> nbrNonFood;

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

            this.PopulateSession(stepsTraining1, stepsBlock1, bufST1, bufSB1 );
            this.PopulateSession(stepsTraining2, stepsBlock2, bufST2, bufSB2 );

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
        protected void PopulateSession(SSTSession sessionTraining, SSTSession sessionBlock, DynamicBuffer<StepStimulus> bufTraining, DynamicBuffer<StepStimulus> bufTBlock)
        {
            this.nbrFood = new NativeList<sbyte>(Const.NBR_IMAGE, Allocator.TempJob);
            this.nbrNonFood = new NativeList<sbyte>(Const.NBR_IMAGE, Allocator.TempJob);

            StepStimulus[] stepsTraining;
            StepStimulus[] stepsBlock;

            sessionTraining.activeStep = 0;
            sessionBlock.activeStep = 0;

            sessionTraining.nbrOfStep = Const.NBR_STIMULUS_TRAINING;
            stepsTraining = new StepStimulus[Const.NBR_STIMULUS_TRAINING];

            sessionBlock.nbrOfStep = Const.NBR_STIMULUS_BLOCK;
            stepsBlock = new StepStimulus[Const.NBR_STIMULUS_BLOCK];

            this.nbrOfStopSignalTraining = (sbyte)(sessionTraining.nbrOfStep / 4);
            this.nbrOfStopSignalBlock = (sbyte)(sessionBlock.nbrOfStep / 4);

            // Fill the two list with int from 1 to 70
            for (sbyte i = 1; i <= Const.NBR_IMAGE; i++) {
                nbrFood.Add(i);
                nbrNonFood.Add(i);
            }

            // Create the good number of step 
            for (sbyte i = 0; i < sessionTraining.nbrOfStep; i++) {
                stepsTraining[i] = CreateStep(i, true);
            }

            for (sbyte i = 0; i < sessionBlock.nbrOfStep; i++) {
                stepsBlock[i] = CreateStep(i, false);
            }

            // Shuffle the list of steps
            this.Shuffle(stepsTraining);
            this.Shuffle(stepsBlock);

            for (sbyte i = 0; i < sessionTraining.nbrOfStep; i++) {
                StepStimulus s = stepsTraining[i];
                s.index = i;
                bufTraining.Add(s);
            }

            for (sbyte i = 0; i < sessionBlock.nbrOfStep; i++) {
                StepStimulus s = stepsBlock[i];
                s.index = i;
                bufTBlock.Add(s);
            }

            nbrFood.Dispose();
            nbrNonFood.Dispose();
        }

        // Create one step
        private StepStimulus CreateStep(int i, bool isTraining)
        {
            bool isFood = i % 2 == 0 ? true: false ; // Half food, half non-food
            sbyte newIndexImg = PickNumber(isFood); // Get the image for the food or non-food
            sbyte nbrOfStopSignal = isTraining ? this.nbrOfStopSignalTraining : this.nbrOfStopSignalBlock;

            StepStimulus s = new StepStimulus {
                //s.index = i;
                indexImg = newIndexImg,
                isFood = isFood,
                isStopSignal = i < nbrOfStopSignal ? true : false, // Get the number of stop signal needed
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
        protected sbyte PickNumber(bool isFood)
        {
            sbyte nbr = -1;

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
