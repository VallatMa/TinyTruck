using Unity.Entities;

namespace Tiny2D
{
    [GenerateAuthoringComponent]
    public struct ChangeScene : IComponentData
    {
        public SceneName Value;
    }
}