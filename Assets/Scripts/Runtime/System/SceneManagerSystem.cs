using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Tiny2D
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SceneManagerSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities
              .WithStructuralChanges()
              .ForEach((Entity entity, ref SceneManager sceneManager, in ChangeScene changeScene) => {
                  DestroyAllSceneEntities(ecb);
                  sceneManager.CurrentSceneType = changeScene.Value;

                  if (sceneManager.CurrentSceneType == SceneName.Gameplay) {
                      EntityManager.Instantiate(sceneManager.GameplayScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.InstTrainingFL) {
                      EntityManager.Instantiate(sceneManager.InstTrainingFLScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.InstTrainingFR) {
                      EntityManager.Instantiate(sceneManager.InstTrainingFRScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.InstGameFL) {
                      EntityManager.Instantiate(sceneManager.InstGameFLScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.InstGameFR) {
                      EntityManager.Instantiate(sceneManager.InstGameFRScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.Menu) {
                      EntityManager.Instantiate(sceneManager.MenuScene);
                  } else if (sceneManager.CurrentSceneType == SceneName.End) {
                      EntityManager.Instantiate(sceneManager.EndScene);
                  }

                  ecb.RemoveComponent<ChangeScene>(entity);
              }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
            return inputDeps;
        }

        // Destroy a scene with all the entities in it
        private void DestroyAllSceneEntities(EntityCommandBuffer ecb)
        {
            EntityQuery eq = GetEntityQuery(ComponentType.ReadOnly<ScenePrefabTag>());

            var scenePrefabs = eq.ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < scenePrefabs.Length; i++) {
                DestroyAllLinkedEntities(scenePrefabs[i], ecb);
            }

            scenePrefabs.Dispose();
        }

        // Destroy the entities in a scene
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
    }
}