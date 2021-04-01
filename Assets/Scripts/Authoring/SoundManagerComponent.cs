using Unity.Entities;
using UnityEngine;

public class SoundManagerComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public AudioClip clip;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Unity.Tiny.Audio.AudioSource() {
            clip = conversionSystem.GetPrimaryEntity(clip),
            volume  = 1,
            loop = false
        });
    }

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public class DeclareAudioClipReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((SoundManagerComponent sma) => {
                if (sma.clip == null) { return; }
                DeclareReferencedAsset(sma.clip);
            });
        }
    }
}