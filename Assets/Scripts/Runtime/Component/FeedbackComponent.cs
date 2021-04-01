using Unity.Entities;

[GenerateAuthoringComponent]
public struct FeedbackComponent : IComponentData
{
    public bool isOK;
    public float Duration;
    public float DurationLeft;
}


