using Unity.Entities;

namespace Tiny2D
{
    [GenerateAuthoringComponent]
    public struct SceneManager : IComponentData
    {
        public SceneName CurrentSceneType;
        public Entity GameplayScene;
        public Entity InstTrainingFLScene;
        public Entity InstTrainingFRScene;
        public Entity InstGameFLScene;
        public Entity InstGameFRScene;
        public Entity MenuScene;
        public Entity EndScene;

    }
}