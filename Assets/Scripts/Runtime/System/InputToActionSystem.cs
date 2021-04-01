using Tiny2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.U2D.Entities.Physics;

namespace Assets.Scripts.Runtime
{
    class InputToActionSystem : JobComponentSystem
    {
        public ComponentDataFromEntity<ButtonLeftRight> myTypeFromEntity;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            RequireSingletonForUpdate<ActiveInput>(); // Witout that entity created, onUpdate won't work
            myTypeFromEntity = GetComponentDataFromEntity<ButtonLeftRight>(true);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var input = World.GetExistingSystem<Unity.Tiny.Input.InputSystem>();
            var physicsWorld = World.GetExistingSystem<PhysicsWorldSystem>().PhysicsWorld;

            if (InputUtil.GetInputDown(input)) {
                var inputPos = CameraUtil.ScreenPointToViewportPoint(EntityManager, InputUtil.GetInputPosition(input));
                
                var activeInput = GetSingleton<ActiveInput>();

                var clickOnButton = new ClickOnButton() {
                    isButton = 0
                };

                if(inputPos.x > 0) {
                    clickOnButton.isButton = 2;
                } else {
                    clickOnButton.isButton = 1;

                }
                activeInput.value = clickOnButton;
                //activeInput.value = GetIsButtonClick(ref physicsWorld, ref inputPos, ref myTypeFromEntity);

                SetSingleton(activeInput);
            }

            return inputDeps;
        }

        private static ClickOnButton GetIsButtonClick(ref PhysicsWorld physicsWorld, ref float2 worldPos, ref ComponentDataFromEntity<ButtonLeftRight> myTypeFromEntity)
        {
            var pointInput = new OverlapPointInput() {
                Position = worldPos,
                Filter = CollisionFilter.Default
            };

            var clickOnButton = new ClickOnButton() {
                isButton = 0
            };
            
            if (physicsWorld.OverlapPoint(pointInput, out var overlapPointHit)) {
                var entity = physicsWorld.AllBodies[overlapPointHit.PhysicsBodyIndex].Entity;
                Debug.Log("Entity index overlap point hit: " + entity.Index);
                if (myTypeFromEntity.HasComponent(entity)) {
                    ButtonLeftRight myType = myTypeFromEntity[entity];
                    if (myType.isLeft) {
                        Debug.Log("Click on left button");
                        clickOnButton.isButton = 1;
                    } else {
                        Debug.Log("Click on right button");
                        clickOnButton.isButton = 2;
                    }
                }
            }
            
            return clickOnButton;
        }
    }
}
