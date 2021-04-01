using Unity.Entities;
using Unity.Mathematics;

namespace Tiny2D
{
    public struct ResetPosition : IComponentData
    {
        public float2 position;
    }

    // The struct that hold all the prefab for the houses
    public struct SpawnerHolder : IComponentData
    {
        public Entity roadPrefab;

        public Entity house1PrefabL;
        public Entity house2PrefabL;
        public Entity house3PrefabL;
        public Entity house4PrefabL;
        public Entity house5PrefabL;

        public Entity house1PrefabR;
        public Entity house2PrefabR;
        public Entity house3PrefabR;
        public Entity house4PrefabR;
        public Entity house5PrefabR;

        public Entity nature1Prefab;
        public Entity nature2Prefab;
        public Entity nature3Prefab;
        public Entity nature4Prefab;
    }

    // The struct that hold all the prefab for the feedback
    public struct FeedbackHolder : IComponentData
    {
        public Entity feedbackOK;
        public Entity feedbackKO;
    }

    public struct ClickOnButton : IComponentData
    {
        public int isButton; // 0 = no button, 1 = leftArrow, 2 = rightArrow
    }

    public struct ActiveInput : IComponentData
    {
        public ClickOnButton value;
    }

    public struct ButtonLeftRight : IComponentData
    {
        public bool isLeft;
    }

    // The struct used for a step
    public struct StepStimulus : IBufferElementData
    {
        public int index;
        public bool isFood;
        public bool isStopSignal;
        public bool isSuccess;
        public bool isTimeOut;
        public bool isAnswered;
        public int indexImg;
        public float timeStartStimulus;
        public float timeEndStimulus;
        public float reactionTime;
        public float averageReactionTime;
        public float ssd;
        public float ssrt;
    }

    // The struct for a session
    public struct SSTSession : IComponentData
    {
        public bool isSessionActive;
        public bool isSessionDone;
        public int activeStep;
        public int nbrOfStep;
        public int nbrOfFalse;
        public int nbrOfTrue;
        public bool isFoodLeft;
        public float actualSSD;
    }

    // The struct that hold all the sessions for the game
    public struct SSTHolder : IComponentData
    {
        public int nbrSessionDone;
        public Entity stepsTraining1; // 1
        public Entity stepsBlock1;    // 2
        public Entity stepsTraining2; // 3
        public Entity stepsBlock2;    // 4
    }

    // The struct used for iterating between steps and to spawn it
    public struct SSTIterator : IComponentData
    {
        public bool isRunning;
        public bool isWaitingForClickOrTimeOut;

        public float spawnTimeStimulus;
        public float responseTimeStimulus;

        public StepStimulus actualStep;
        public Entity actualSessionEntity;
    }
}