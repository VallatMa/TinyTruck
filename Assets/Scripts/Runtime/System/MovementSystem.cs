using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Tiny2D
{
    [AlwaysSynchronizeSystem]
    public class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((
                Entity e,
                ref Translation trans, in Movement moveForward) => {
                    trans.Value += moveForward.speed * moveForward.direction * deltaTime;// * math.forward(rot.Value);
                }).Schedule();
        }
    }
}