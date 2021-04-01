using UnityEngine;
using Unity.Entities;

namespace Tiny2D
{
    [ConverterVersion("Tiny2D", 1)]
    public class ButtonComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool isArrowButton;
        public bool isArrowLeft;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (isArrowButton) {
                dstManager.AddComponentData(entity, new ButtonLeftRight() {
                    isLeft = isArrowLeft
                });
            }
        }
    }
}