using Unity.Entities;

namespace Tiny2D
{
    [GenerateAuthoringComponent]
    public struct SceneManager : IComponentData
    {
        public SceneName CurrentSceneType;
        public Entity GameplayScene;
        public Entity MenuScene;
        public Entity EndScene;
        public Entity Story1;
        public Entity Story2;
        public Entity InstructionL1;
        public Entity InstructionR1;
        public Entity InstructionLR;
        public Entity InstructionL2;
        public Entity InstructionR2;
        public Entity InstructionFast; // New
        public Entity GameStructure1L1;
        public Entity GameStructure1L2;
        public Entity GameStructure2L1;
        public Entity GameStructure2L2;
        public Entity GameStructure1R1;
        public Entity GameStructure1R2;
        public Entity GameStructure2R1;
        public Entity GameStructure2R2;

    }
}