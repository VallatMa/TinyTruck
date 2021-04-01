using Unity.Entities;
using Unity.Mathematics;

namespace Tiny2D
{
    public struct Movement : IComponentData
    {
        public float speed;
        public float3 direction;
    }
}
