using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Runtime.Component
{

    // The struct that hold all the prefab for the houses
    [GenerateAuthoringComponent]
    public struct SpawnerHolder : IComponentData
    {
        public Entity roadPrefab;

        public Entity house1PrefabL;
        public Entity house2PrefabL;
        public Entity house3PrefabL;
        public Entity house4PrefabL;
        public Entity house5PrefabL;

        public Entity house1PrefabR;
        public Entity house2PrefabR;
        public Entity house3PrefabR;
        public Entity house4PrefabR;
        public Entity house5PrefabR;

        public Entity nature1Prefab;
        public Entity nature2Prefab;
        public Entity nature3Prefab;
        public Entity nature4Prefab;
    }
}
