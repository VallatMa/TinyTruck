using Assets.Scripts.Runtime;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace Tiny2D
{
    // Spawning System for Houses
    public class SpawningHouseSystem : ComponentSystem
    {
        private Random randomGenerator;
        private float spawnTimeHouse;

        protected override void OnCreate()
        {
            randomGenerator = new Random();
            randomGenerator.InitState();
        }

        protected override void OnUpdate()
        {
            spawnTimeHouse -= Time.DeltaTime;

            if (spawnTimeHouse <= 0f) {

                bool isLeft = randomGenerator.NextBool();
                bool isHouse = randomGenerator.NextBool();
                int houseModel = 1, natureModel = 1;

                if (isHouse) {
                    spawnTimeHouse = Const.SPAWN_DELAY_HOUSE;
                    if (isLeft) {
                        houseModel = randomGenerator.NextInt(1, Const.NBR_HOUSES_LEFT);
                    } else {
                        houseModel = randomGenerator.NextInt(1, Const.NBR_HOUSES_RIGTH);
                    }
                    //Debug.Log("House model: " + houseModel + " isLeft: " + isLeft);
                } else {
                    spawnTimeHouse = Const.SPAWN_DELAY_NATURE;
                    natureModel = randomGenerator.NextInt(1, Const.NBR_NATURE);
                }


                Entities.ForEach((ref SpawnerHolder s) => {

                    Entity instantiate = s.house3PrefabL;

                    if (isHouse) {
                        if (isLeft) {
                            switch (houseModel) {
                                case 1:
                                    instantiate = s.house1PrefabL;
                                    break;
                                case 2:
                                    instantiate = s.house2PrefabL;
                                    break;
                                case 3:
                                    instantiate = s.house3PrefabL;
                                    break;
                                case 4:
                                    instantiate = s.house4PrefabL;
                                    break;
                                case 5:
                                    instantiate = s.house4PrefabL;
                                    break;
                            }
                        } else {
                            switch (houseModel) {
                                case 1:
                                    instantiate = s.house1PrefabR;
                                    break;
                                case 2:
                                    instantiate = s.house2PrefabR;
                                    break;
                                case 3:
                                    instantiate = s.house3PrefabR;
                                    break;
                                case 4:
                                    instantiate = s.house4PrefabR;
                                    break;
                                case 5:
                                    instantiate = s.house4PrefabR;
                                    break;
                            }
                        }
                    } else {
                        switch (natureModel) {
                            case 1:
                                instantiate = s.nature1Prefab;
                                break;
                            case 2:
                                instantiate = s.nature2Prefab;
                                break;
                            case 3:
                                instantiate = s.nature3Prefab;
                                break;
                            case 4:
                                instantiate = s.nature4Prefab;
                                break;
                        }
                    }

                    Entity h = EntityManager.Instantiate(instantiate);

                    EntityManager.SetComponentData(h,
                        new Translation { Value = new float3(isLeft ? -1.56f : 1.86f, 7f, 0f) } // value calculated in editor
                    );

                });

            }


        }
    }


}
