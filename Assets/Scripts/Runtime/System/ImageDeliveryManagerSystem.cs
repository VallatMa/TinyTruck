using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace Tiny2D
{
    //[AlwaysSynchronizeSystem]

    //[UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ImageDeliveryManagerSystem : ComponentSystem
    {

        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<SSTIterator>();
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            var sstIterator = GetSingleton<SSTIterator>();

            Entities
                .ForEach((ref ImageManager iM) => {

                    EntityQuery eq = GetEntityQuery(ComponentType.ReadOnly<ImagePrefabTag>());
                    var listImg = eq.ToEntityArray(Allocator.TempJob);

                    if (sstIterator.isWaitingForClickOrTimeOut) {

                        // Add on top of the waiting the image of the food/nfood
                        if (listImg.Length == 0) {
                            Entity prefab = this.GetImage(iM, sstIterator.actualStep.indexImg, sstIterator.actualStep.isFood);
                            Entity h = EntityManager.Instantiate(prefab);
                            EntityManager.SetComponentData(h, new Translation { Value = new float3(0f, 3.694f, 0f) });
                        }

                    } else {

                        var ecb = new EntityCommandBuffer(Allocator.TempJob);
                        if (listImg.Length != 0)
                            ecb.DestroyEntity(listImg[0]);
                        ecb.DestroyEntity(eq);
                        ecb.Playback(EntityManager);
                        ecb.Dispose();
                    }
                    listImg.Dispose();
                });

        }

        private void DestroyAllSceneEntities(EntityCommandBuffer ecb)
        {
            EntityQuery eq = GetEntityQuery(ComponentType.ReadOnly<ScenePrefabTag>());

            var scenePrefabs = eq.ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < scenePrefabs.Length; i++) {
                DestroyAllLinkedEntities(scenePrefabs[i], ecb);
            }

            scenePrefabs.Dispose();
        }

        private void DestroyAllLinkedEntities(Entity sceneEntity, EntityCommandBuffer ecb)
        {
            if (EntityManager.HasComponent<LinkedEntityGroup>(sceneEntity)) {
                var buffer = EntityManager.GetBuffer<LinkedEntityGroup>(sceneEntity);
                var linkedEntities = buffer.ToNativeArray(Allocator.TempJob);

                for (int i = 0; i < linkedEntities.Length; i++) {
                    ecb.DestroyEntity(linkedEntities[i].Value);
                }

                linkedEntities.Dispose();
            }
        }

        private Entity GetImage(ImageManager iM, int num, bool isFood)
        {
            Debug.Log("Image num: " + num + ", isFood: " + isFood);
            Entity r = new Entity();
            if (isFood) {
                switch (num) {
                    case 1: r = iM.F1; break;
                    case 2: r = iM.F2; break;
                    case 3: r = iM.F3; break;
                    case 4: r = iM.F4; break;
                    case 5: r = iM.F5; break;
                    case 6: r = iM.F6; break;
                    case 7: r = iM.F7; break;
                    case 8: r = iM.F8; break;
                    case 9: r = iM.F9; break;
                    case 10: r = iM.F10; break;
                    case 11: r = iM.F12; break;
                    case 12: r = iM.F12; break;
                    case 13: r = iM.F13; break;
                    case 14: r = iM.F14; break;
                    case 15: r = iM.F15; break;
                    case 16: r = iM.F16; break;
                    case 17: r = iM.F17; break;
                    case 18: r = iM.F18; break;
                    case 19: r = iM.F19; break;
                    case 20: r = iM.F20; break;
                    case 21: r = iM.F21; break;
                    case 22: r = iM.F22; break;
                    case 23: r = iM.F23; break;
                    case 24: r = iM.F24; break;
                    case 25: r = iM.F25; break;
                    case 26: r = iM.F26; break;
                    case 27: r = iM.F27; break;
                    case 28: r = iM.F28; break;
                    case 29: r = iM.F29; break;
                    case 30: r = iM.F30; break;
                    case 31: r = iM.F31; break;
                    case 32: r = iM.F32; break;
                    case 33: r = iM.F33; break;
                    case 34: r = iM.F34; break;
                    case 35: r = iM.F35; break;
                    case 36: r = iM.F36; break;
                    case 37: r = iM.F37; break;
                    case 38: r = iM.F38; break;
                    case 39: r = iM.F39; break;
                    case 40: r = iM.F40; break;
                    case 41: r = iM.F41; break;
                    case 42: r = iM.F42; break;
                    case 43: r = iM.F43; break;
                    case 44: r = iM.F44; break;
                    case 45: r = iM.F45; break;
                    case 46: r = iM.F46; break;
                    case 47: r = iM.F47; break;
                    case 48: r = iM.F48; break;
                    case 49: r = iM.F49; break;
                    case 50: r = iM.F50; break;
                    case 51: r = iM.F51; break;
                    case 52: r = iM.F52; break;
                    case 53: r = iM.F53; break;
                    case 54: r = iM.F54; break;
                    case 55: r = iM.F55; break;
                    case 56: r = iM.F56; break;
                    case 57: r = iM.F57; break;
                    case 58: r = iM.F58; break;
                    case 59: r = iM.F59; break;
                    case 60: r = iM.F60; break;
                    case 61: r = iM.F61; break;
                    case 62: r = iM.F62; break;
                    case 63: r = iM.F63; break;
                    case 64: r = iM.F64; break;

                }
            } else {
                switch (num) {
                    case 1: r = iM.FN1; break;
                    case 2: r = iM.FN2; break;
                    case 3: r = iM.FN3; break;
                    case 4: r = iM.FN4; break;
                    case 5: r = iM.FN5; break;
                    case 6: r = iM.FN6; break;
                    case 7: r = iM.FN7; break;
                    case 8: r = iM.FN8; break;
                    case 9: r = iM.FN9; break;
                    case 10: r = iM.FN10; break;
                    case 11: r = iM.FN11; break;
                    case 12: r = iM.FN12; break;
                    case 13: r = iM.FN13; break;
                    case 14: r = iM.FN14; break;
                    case 15: r = iM.FN15; break;
                    case 16: r = iM.FN16; break;
                    case 17: r = iM.FN17; break;
                    case 18: r = iM.FN18; break;
                    case 19: r = iM.FN19; break;
                    case 20: r = iM.FN20; break;
                    case 21: r = iM.FN21; break;
                    case 22: r = iM.FN22; break;
                    case 23: r = iM.FN23; break;
                    case 24: r = iM.FN24; break;
                    case 25: r = iM.FN25; break;
                    case 26: r = iM.FN26; break;
                    case 27: r = iM.FN27; break;
                    case 28: r = iM.FN28; break;
                    case 29: r = iM.FN29; break;
                    case 30: r = iM.FN30; break;
                    case 31: r = iM.FN31; break;
                    case 32: r = iM.FN32; break;
                    case 33: r = iM.FN33; break;
                    case 34: r = iM.FN34; break;
                    case 35: r = iM.FN35; break;
                    case 36: r = iM.FN36; break;
                    case 37: r = iM.FN37; break;
                    case 38: r = iM.FN38; break;
                    case 39: r = iM.FN39; break;
                    case 40: r = iM.FN40; break;
                    case 41: r = iM.FN41; break;
                    case 42: r = iM.FN42; break;
                    case 43: r = iM.FN43; break;
                    case 44: r = iM.FN44; break;
                    case 45: r = iM.FN45; break;
                    case 46: r = iM.FN46; break;
                    case 47: r = iM.FN47; break;
                    case 48: r = iM.FN48; break;
                    case 49: r = iM.FN49; break;
                    case 50: r = iM.FN50; break;
                    case 51: r = iM.FN51; break;
                    case 52: r = iM.FN52; break;
                    case 53: r = iM.FN53; break;
                    case 54: r = iM.FN54; break;
                    case 55: r = iM.FN55; break;
                    case 56: r = iM.FN56; break;
                    case 57: r = iM.FN57; break;
                    case 58: r = iM.FN58; break;
                    case 59: r = iM.FN59; break;
                    case 60: r = iM.FN60; break;
                    case 61: r = iM.FN61; break;
                    case 62: r = iM.FN62; break;
                    case 63: r = iM.FN63; break;
                    case 64: r = iM.FN64; break;
                }
            }

            return r;
        }
    }
}