using Assets.Scripts.Runtime;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Tiny;

namespace Tiny2D
{
    class ChangeSceneSystem : JobComponentSystem
    {
        private float clickDelayForChangingScene = Const.CHANGE_DELAY_SCENE;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            RequireSingletonForUpdate<SceneManager>();
            RequireSingletonForUpdate<ActiveInput>();
            RequireSingletonForUpdate<SSTIterator>();
            RequireSingletonForUpdate<SSTHolder>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ai = GetSingleton<ActiveInput>();
            int buttonId = ai.value.isButton;

            clickDelayForChangingScene -= Time.DeltaTime;

            if (buttonId != 0 && clickDelayForChangingScene <= 0f) {
                Entity sceneManagerEntity = GetSingletonEntity<SceneManager>();
                var sstIterator = GetSingleton<SSTIterator>();
                var sstHolder = GetSingleton<SSTHolder>();
                var ecb = new EntityCommandBuffer(Allocator.Temp);

                SSTSession actualSession = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                Debug.Log("[Next session] nbrOfStep: " + actualSession.nbrOfStep + " isFoodLeft: " + actualSession.isFoodLeft);

                // Select the next instruction scene
                Entities
                .WithoutBurst()
                .ForEach((Entity entity, in SceneManager sm) => {
                    switch (sm.CurrentSceneType) {
                        case SceneName.Menu:
                            this.ChangeScene(ecb, SceneName.Story1, sceneManagerEntity);
                            break;
                        case SceneName.Story1:
                            if (actualSession.isFoodLeft) {
                                this.ChangeScene(ecb, SceneName.InstructionL1, sceneManagerEntity);
                            } else {
                                this.ChangeScene(ecb, SceneName.InstructionR1, sceneManagerEntity);
                            }
                            break;
                        case SceneName.InstructionL1:
                            this.ChangeScene(ecb, SceneName.InstructionLR, sceneManagerEntity);
                            break;
                        case SceneName.InstructionR1:
                            this.ChangeScene(ecb, SceneName.InstructionLR, sceneManagerEntity);
                            break;
                        case SceneName.InstructionLR:
                            if (actualSession.isFoodLeft) {
                                this.ChangeScene(ecb, SceneName.GameStructure1L1, sceneManagerEntity);
                            } else {
                                this.ChangeScene(ecb, SceneName.GameStructure1R1, sceneManagerEntity);
                            }
                            break;
                        case SceneName.Story2:
                            if (actualSession.isFoodLeft) {
                                this.ChangeScene(ecb, SceneName.InstructionL2, sceneManagerEntity);
                            } else {
                                this.ChangeScene(ecb, SceneName.InstructionR2, sceneManagerEntity);
                            }
                            break;
                        case SceneName.InstructionL2:
                            this.ChangeScene(ecb, SceneName.GameStructure2L1, sceneManagerEntity);

                            break;
                        case SceneName.InstructionR2:

                            this.ChangeScene(ecb, SceneName.GameStructure2R1, sceneManagerEntity);
                            break;
                        case SceneName.Gameplay:
                            if (sstHolder.nbrSessionDone == 1) {
                                if (actualSession.isFoodLeft) {
                                    this.ChangeScene(ecb, SceneName.GameStructure1L2, sceneManagerEntity);
                                } else {
                                    this.ChangeScene(ecb, SceneName.GameStructure1R2, sceneManagerEntity);
                                }
                            }

                            if (sstHolder.nbrSessionDone == 2) {
                                this.ChangeScene(ecb, SceneName.Story2, sceneManagerEntity);
                            }

                            if (sstHolder.nbrSessionDone == 3) {
                                if (actualSession.isFoodLeft) {
                                    this.ChangeScene(ecb, SceneName.GameStructure2L2, sceneManagerEntity);
                                } else {
                                    this.ChangeScene(ecb, SceneName.GameStructure2R2, sceneManagerEntity);
                                }
                            }

                            if (sstHolder.nbrSessionDone == 4)
                                this.ChangeScene(ecb, SceneName.End, sceneManagerEntity);
                            break;

                        case SceneName.GameStructure1L2:
                        case SceneName.GameStructure2L2:
                        case SceneName.GameStructure1R2:
                        case SceneName.GameStructure2R2:
                            this.ChangeScene(ecb, SceneName.InstructionFast, sceneManagerEntity); // New
                            break;

                        case SceneName.InstructionFast:
                        case SceneName.GameStructure1L1:
                        case SceneName.GameStructure2L1:
                        case SceneName.GameStructure1R1:
                        case SceneName.GameStructure2R1:
                            this.ChangeScene(ecb, SceneName.Gameplay, sceneManagerEntity);
                            this.SetSessionRunning(sstIterator, true);
                            break;
                    }

                }).Run();

                this.clickDelayForChangingScene = Const.CHANGE_DELAY_SCENE;
                SetSingleton(new ActiveInput());
                ecb.Playback(EntityManager);
                ecb.Dispose();
            } else {
                SetSingleton(new ActiveInput());
            }

            return default;
        }

        private void ChangeScene(EntityCommandBuffer ecb, SceneName sn, Entity sm)
        {
            ecb.AddComponent(sm, new ChangeScene() {
                Value = sn
            });
        }

        private void SetSessionRunning(SSTIterator sstI, bool isRunning)
        {
            sstI.isRunning = isRunning;
            SetSingleton(sstI);
        }

    }
}
