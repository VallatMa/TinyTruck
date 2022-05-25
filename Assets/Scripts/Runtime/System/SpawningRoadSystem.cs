using Assets.Scripts.Runtime.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace Tiny2D
{
    // Spawning System for the Road/BackGround
    public class SpawningRoadSystem : ComponentSystem
    {
        private Random randomGenerator;
        private float spawnTimeRoad;
        private int nbrOfRoadPrefab = 3;

        protected override void OnCreate()
        {
            randomGenerator = new Random();
            randomGenerator.InitState();
        }

        protected override void OnUpdate()
        {
            spawnTimeRoad -= Time.DeltaTime;

            bool needToSpawn = false;
            int countRoad = 0;
            float displacement = 0f;

            Entities.ForEach((Entity e, ref RoadTag s) => {
                Translation t = EntityManager.GetComponentData<Translation>(e);
                countRoad++;
                if (t.Value.y <= 0 && countRoad < nbrOfRoadPrefab) {
                    displacement = t.Value.y;
                    needToSpawn = true;
                }
            });

            //Debug.Log("RoadCount: " + countRoad);

            if (countRoad < nbrOfRoadPrefab) {
                displacement += 18.8f; // Value calculated in the editor

                Entities.ForEach((ref SpawnerHolder s) => {
                    if (needToSpawn) {
                        Entity r = EntityManager.Instantiate(s.roadPrefab);
                        EntityManager.SetComponentData(r,
                            new Translation { Value = new float3(0, displacement, 0f) }
                        );
                    }
                });
            }
        }


    }
}
