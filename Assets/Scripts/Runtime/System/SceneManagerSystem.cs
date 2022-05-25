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
                  sceneManager.CurrentSceneType = changeScene.Value; // get the new scene fromn the in changescene

                  // Select the scene on the sceneManager with the Enum
                  if (sceneManager.CurrentSceneType == SceneName.Gameplay)
                      EntityManager.Instantiate(sceneManager.GameplayScene);
                  if (sceneManager.CurrentSceneType == SceneName.Menu)
                      EntityManager.Instantiate(sceneManager.MenuScene);
                  if (sceneManager.CurrentSceneType == SceneName.End)
                      EntityManager.Instantiate(sceneManager.EndScene);

                  if (sceneManager.CurrentSceneType == SceneName.Story1)
                      EntityManager.Instantiate(sceneManager.Story1);
                  if (sceneManager.CurrentSceneType == SceneName.Story2)
                      EntityManager.Instantiate(sceneManager.Story2);

                  if (sceneManager.CurrentSceneType == SceneName.InstructionL1)
                      EntityManager.Instantiate(sceneManager.InstructionL1);
                  if (sceneManager.CurrentSceneType == SceneName.InstructionR1)
                      EntityManager.Instantiate(sceneManager.InstructionR1);
                  if (sceneManager.CurrentSceneType == SceneName.InstructionLR)
                      EntityManager.Instantiate(sceneManager.InstructionLR);
                  if (sceneManager.CurrentSceneType == SceneName.InstructionL2)
                      EntityManager.Instantiate(sceneManager.InstructionL2);
                  if (sceneManager.CurrentSceneType == SceneName.InstructionR2)
                      EntityManager.Instantiate(sceneManager.InstructionR2);

                  if (sceneManager.CurrentSceneType == SceneName.InstructionFast) // New
                      EntityManager.Instantiate(sceneManager.InstructionFast); // New

                  if (sceneManager.CurrentSceneType == SceneName.GameStructure1L1)
                      EntityManager.Instantiate(sceneManager.GameStructure1L1);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure1L2)
                      EntityManager.Instantiate(sceneManager.GameStructure1L2);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure2L1)
                      EntityManager.Instantiate(sceneManager.GameStructure2L1);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure2L2)
                      EntityManager.Instantiate(sceneManager.GameStructure2L2);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure1R1)
                      EntityManager.Instantiate(sceneManager.GameStructure1R1);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure1R2)
                      EntityManager.Instantiate(sceneManager.GameStructure1R2);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure2R1)
                      EntityManager.Instantiate(sceneManager.GameStructure2R1);
                  if (sceneManager.CurrentSceneType == SceneName.GameStructure2R2)
                      EntityManager.Instantiate(sceneManager.GameStructure2R2);

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