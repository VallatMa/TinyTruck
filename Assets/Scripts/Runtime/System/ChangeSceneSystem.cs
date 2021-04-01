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

                SceneName nextScene;
                SSTSession actualSession = EntityManager.GetComponentData<SSTSession>(sstIterator.actualSessionEntity);
                Debug.Log("[Next session] nbrOfStep: " + actualSession.nbrOfStep + " isFoodLeft: " + actualSession.isFoodLeft);

                // Select the next instruction scene
                if (actualSession.isFoodLeft) {
                    if (actualSession.nbrOfStep == Const.NBR_STIMULUS_TRAINING) {
                        nextScene = SceneName.InstTrainingFL;
                    } else {
                        nextScene = SceneName.InstGameFL;
                    }
                } else {
                    if (actualSession.nbrOfStep == Const.NBR_STIMULUS_TRAINING) {
                        nextScene = SceneName.InstTrainingFR;
                    } else {
                        nextScene = SceneName.InstGameFR;
                    }
                }

                // Show the end scene
                if (sstHolder.nbrSessionDone == 4)
                    nextScene = SceneName.End;

                Entities
                .WithoutBurst()
                .ForEach((Entity entity, in SceneManager sm) => {
                    switch (sm.CurrentSceneType) {
                        case SceneName.Menu:
                            this.ChangeScene(ecb, nextScene, sceneManagerEntity);
                            break;
                        case SceneName.InstTrainingFL:
                            this.ChangeScene(ecb, SceneName.Gameplay, sceneManagerEntity);
                            this.SetSessionRunning(sstIterator, true);
                            break;
                        case SceneName.InstTrainingFR:
                            this.ChangeScene(ecb, SceneName.Gameplay, sceneManagerEntity);
                            this.SetSessionRunning(sstIterator, true);
                            break;
                        case SceneName.InstGameFL:
                            this.ChangeScene(ecb, SceneName.Gameplay, sceneManagerEntity);
                            this.SetSessionRunning(sstIterator, true);
                            break;
                        case SceneName.InstGameFR:
                            this.ChangeScene(ecb, SceneName.Gameplay, sceneManagerEntity);
                            this.SetSessionRunning(sstIterator, true);
                            break;
                    }

                    // Button ID 5 is for changin from gameplay to something else
                    if (buttonId == 5) {
                        this.ChangeScene(ecb, nextScene, sceneManagerEntity);
                        this.SetSessionRunning(sstIterator, false);
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
