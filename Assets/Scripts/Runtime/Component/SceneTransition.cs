using Unity.Entities;
using Unity.Mathematics;

namespace Tiny2D
{
    [GenerateAuthoringComponent]
    public struct SceneTransition : IComponentData
    {
        public float duration;
        public float durationLeft;
        public float3 topPosition;
        public float3 botPosition;
    }
}