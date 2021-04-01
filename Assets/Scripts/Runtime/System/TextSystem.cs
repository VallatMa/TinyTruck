using Tiny2D;
using Unity.Entities;
using Unity.Tiny.Text;

namespace Assets.Scripts.Runtime.Pure
{
    public class TextSystem : SystemBase
    {
        private Entity TitleInstructionEntity;
        private Entity InstructionsEntity;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<TitleInstructionText>();
            RequireSingletonForUpdate<InstructionsText>();
            base.OnCreate();
        }

        protected override void OnStartRunning()
        {
            TitleInstructionEntity = GetSingletonEntity<TitleInstructionText>();
            InstructionsEntity = GetSingletonEntity<InstructionsText>();
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            TextLayout.SetEntityTextRendererString(EntityManager, TitleInstructionEntity, "Title");
            TextLayout.SetEntityTextRendererString(EntityManager, InstructionsEntity, "Test blablabla");
        }
        
    }
}
