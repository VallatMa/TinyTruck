using Unity.Entities;

public struct SoundManager : IComponentData
{
    public bool IsMusicEnabled;
    public bool IsSoundEnabled;
    public Entity StopSignal;
}