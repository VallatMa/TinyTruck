using Unity.Entities;
using UnityEngine;

public class SoundManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool IsSoundEnabled = true;
    public bool IsMusicEnabled = true;
    public AudioSource StopSignal;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SoundManager() {
            IsSoundEnabled = IsSoundEnabled,
            IsMusicEnabled = IsMusicEnabled,
            StopSignal = conversionSystem.GetPrimaryEntity(StopSignal)
        });
    }

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public class DeclareAudioClipReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((SoundManagerAuthoring sma) => {
                if (sma.StopSignal == null) { return; }
                DeclareReferencedAsset(sma.StopSignal.clip);
            });
        }
    }
}