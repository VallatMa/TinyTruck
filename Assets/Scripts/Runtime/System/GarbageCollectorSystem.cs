using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Tiny2D
{
    public class GarbageCollectorSystem : SystemBase
    {
        EntityCommandBufferSystem m_Barrier;

        protected override void OnCreate()
        {
            m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_Barrier.CreateCommandBuffer().AsParallelWriter();

            Entities.ForEach((Entity e, int nativeThreadIndex, ref Translation trans) => {
                if (trans.Value.y < -22) {
                    commandBuffer.DestroyEntity(nativeThreadIndex, e);
                }
            }).ScheduleParallel();

            m_Barrier.AddJobHandleForProducer(Dependency);

        }


    }
}
