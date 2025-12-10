using System.Collections.Generic;
using CaveworldFlora;
using RimWorld;
using Verse;

namespace CaveBiome;

public class GenStep_CavePlants : GenStep_Plants
{
    private const float plantMinGrowth = 0.07f;
    private const float plantMaxGrowth = 1.0f;

    public override void Generate(Map map, GenStepParams parms)
    {
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            // Use standard base function.
            base.Generate(map, parms);
            return;
        }

        // Enabled it to avoid error while checking new plant can be spawned nearby in same room.
        map.regionAndRoomUpdater.Enabled = true;
        var wildCavePlants = new List<ThingDef_ClusterPlant>();
        var wildCavePlantsWeighted = new Dictionary<ThingDef_ClusterPlant, float>();
        foreach (var thingDef in map.Biome.AllWildPlants)
        {
            if (thingDef is not ThingDef_ClusterPlant cavePlantDef)
            {
                continue;
            }

            wildCavePlants.Add(cavePlantDef);
            wildCavePlantsWeighted.Add(cavePlantDef,
                map.Biome.CommonalityOfPlant(cavePlantDef) / cavePlantDef.clusterSizeRange.Average);
        }

        var spawnTriesNumber = 10000;
        var failedSpawns = 0;
        for (var tryIndex = 0; tryIndex < spawnTriesNumber; tryIndex++)
        {
            var cavePlantDef = wildCavePlants.RandomElementByWeight(thingDefClusterPlant =>
                wildCavePlantsWeighted[thingDefClusterPlant]);

            var newDesiredClusterSize = cavePlantDef.clusterSizeRange.RandomInRange;
            GenClusterPlantReproduction.TryGetRandomClusterSpawnCell(cavePlantDef, newDesiredClusterSize, false,
                map, out var spawnCell); // Ignore temperature condition.
            if (spawnCell.IsValid)
            {
                failedSpawns = 0;
                var newPlant = Cluster.SpawnNewClusterAt(map, spawnCell, cavePlantDef, newDesiredClusterSize);
                newPlant.Growth = Rand.Range(ClusterPlant.minGrowthToReproduce, plantMaxGrowth);

                var clusterIsMature = Rand.Value < 0.7f;
                growCluster(newPlant, clusterIsMature);
            }
            else
            {
                failedSpawns++;
                if (failedSpawns >= 50)
                {
                    break;
                }
            }
        }
    }

    private static void growCluster(ClusterPlant plant, bool clusterIsMature)
    {
        var cluster = plant.cluster;
        int seedPlantsNumber;
        if (clusterIsMature)
        {
            seedPlantsNumber = cluster.desiredSize - 1; // The first plant is already spawned.
        }
        else
        {
            seedPlantsNumber = (int)(cluster.desiredSize * Rand.Range(0.25f, 0.75f));
        }

        if (seedPlantsNumber == 0)
        {
            return;
        }

        //for (var seedPlantIndex = 0; seedPlantIndex < seedPlantsNumber; seedPlantIndex++)
        // {
        //     var seedPlant =
        //         GenClusterPlantReproduction.TryGrowCluster(cluster, false); // Ignore temperature condition.
        //     if (seedPlant != null)
        //     {
        //         seedPlant.Growth = Rand.Range(plantMinGrowth, plantMaxGrowth);
        //     }
        //  }

        for (var seedPlantIndex = 0; seedPlantIndex < seedPlantsNumber; seedPlantIndex++)
        {
            var growthRange = new FloatRange(plantMinGrowth, plantMaxGrowth);

            // Ignore temperature condition
            GenClusterPlantReproduction.TryGrowCluster(cluster, false, growthRange);
        }

        if (!clusterIsMature || cluster.plantDef.symbiosisPlantDefEvolution == null)
        {
            return;
        }

        var symbiosisPlant = GenClusterPlantReproduction.TrySpawnNewSymbiosisCluster(cluster);
        if (symbiosisPlant == null)
        {
            return;
        }

        symbiosisPlant.Growth = Rand.Range(plantMinGrowth, plantMaxGrowth);
        growCluster(symbiosisPlant, true);
    }
}