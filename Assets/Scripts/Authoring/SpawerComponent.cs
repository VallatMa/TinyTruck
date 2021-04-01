using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

namespace Tiny2D
{
    [ConverterVersion("Tiny2D", 1)]
    public class SpawerComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject roadPrefab;

        public GameObject house1PrefabL;
        public GameObject house2PrefabL;
        public GameObject house3PrefabL;
        public GameObject house4PrefabL;
        public GameObject house5PrefabL;

        public GameObject house1PrefabR;
        public GameObject house2PrefabR;
        public GameObject house3PrefabR;
        public GameObject house4PrefabR;
        public GameObject house5PrefabR;


        public GameObject nature1PRefab;
        public GameObject nature2PRefab;
        public GameObject nature3PRefab;
        public GameObject nature4PRefab;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SpawnerHolder() {
                roadPrefab = conversionSystem.GetPrimaryEntity(this.roadPrefab),
                house1PrefabL = conversionSystem.GetPrimaryEntity(this.house1PrefabL),
                house2PrefabL = conversionSystem.GetPrimaryEntity(this.house2PrefabL),
                house3PrefabL = conversionSystem.GetPrimaryEntity(this.house3PrefabL),
                house4PrefabL = conversionSystem.GetPrimaryEntity(this.house4PrefabL),
                house5PrefabL = conversionSystem.GetPrimaryEntity(this.house5PrefabL),
                house1PrefabR = conversionSystem.GetPrimaryEntity(this.house1PrefabR),
                house2PrefabR = conversionSystem.GetPrimaryEntity(this.house2PrefabR),
                house3PrefabR = conversionSystem.GetPrimaryEntity(this.house3PrefabR),
                house4PrefabR = conversionSystem.GetPrimaryEntity(this.house4PrefabR),
                house5PrefabR = conversionSystem.GetPrimaryEntity(this.house5PrefabR),
                nature1Prefab = conversionSystem.GetPrimaryEntity(this.nature1PRefab),
                nature2Prefab = conversionSystem.GetPrimaryEntity(this.nature2PRefab),
                nature3Prefab = conversionSystem.GetPrimaryEntity(this.nature3PRefab),
                nature4Prefab = conversionSystem.GetPrimaryEntity(this.nature4PRefab)
            });

        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(roadPrefab);

            referencedPrefabs.Add(house1PrefabL);
            referencedPrefabs.Add(house2PrefabL);
            referencedPrefabs.Add(house3PrefabL);
            referencedPrefabs.Add(house4PrefabL);
            referencedPrefabs.Add(house5PrefabL);

            referencedPrefabs.Add(house1PrefabR);
            referencedPrefabs.Add(house2PrefabR);
            referencedPrefabs.Add(house3PrefabR);
            referencedPrefabs.Add(house4PrefabR);
            referencedPrefabs.Add(house5PrefabR);

            referencedPrefabs.Add(nature1PRefab);
            referencedPrefabs.Add(nature2PRefab);
            referencedPrefabs.Add(nature3PRefab);
            referencedPrefabs.Add(nature4PRefab);
        }
    }
}