using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Tiny2D
{
    [AlwaysSynchronizeSystem]
    public class MovementComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float speed = 1f;
        public float3 direction = new float3(0f, 1f, 0f);
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Movement() {
                speed = this.speed, 
                direction = this.direction
            });
        }
    }
}