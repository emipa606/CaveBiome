using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaveBiome;

public class GenStep_CaveRoof : GenStep
{
    private static int caveWellsNumber;
    private static List<IntVec3> caveWellsPosition;

    public override int SeedPart => 647812558;

    public override void Generate(Map map, GenStepParams genStepParams)
    {
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            // Nothing to do in other biomes.
            return;
        }

        // Compute number of cave wells (5 for standard map 250x250, around 13 for bigest map 400x400).
        caveWellsNumber = Mathf.CeilToInt(map.Size.x * map.Size.z / (float)12500);
        foreach (var cell in map.AllCells)
        {
            Thing thing = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(cell)];
            if (thing != null && thing.def.holdsRoof)
            {
                map.roofGrid.SetRoof(cell, RoofDefOf.RoofRockThick);
            }
            else
            {
                // Spawn cave roof holder.
                GenSpawn.Spawn(Util_CaveBiome.CaveRoofDef, cell, map);
            }
        }

        // Update regions and rooms to be able to use the CanReachMapEdge function to find good cave well spots.
        map.regionAndRoomUpdater.Enabled = true;
        map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();

        // Get cave wells position.
        caveWellsPosition = GetCaveWellsPosition(map);

        // Spawn cave wells.
        // First cave well is always dry (to avoid starting thing scattering errors).
        SpawnDryCaveWellWithAnimalCorpsesAt(map, caveWellsPosition[0]);
        for (var caveWellIndex = 1; caveWellIndex < caveWellsNumber; caveWellIndex++)
        {
            if (Rand.Value < 0.5f)
            {
                // Spawn aqueous cave well.
                SpawnAqueousCaveWellAt(map, caveWellsPosition[caveWellIndex]);
            }
            else if (Rand.Value < 0.9f)
            {
                // Spawn dry cave well + fallen animal corpses.
                SpawnDryCaveWellWithAnimalCorpsesAt(map, caveWellsPosition[caveWellIndex]);
            }
            else
            {
                // Spawn dry cave well + sacrificial stone.
                SpawnDryCaveWellWithRitualStoneAt(map, caveWellsPosition[caveWellIndex]);
            }
        }

        // TODO: should correct null region error? May be due to artificial buildings. River should avoid this.
        // Update regions and rooms now that cave wells are spawned.
        map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
    }

    private static List<IntVec3> GetCaveWellsPosition(Map map)
    {
        const float DistanceBeetweenCaveWells = 40f;

        var positionsList = new List<IntVec3>
        {
            // Reuse PlayerStartSpot defined in river generation.
            MapGenerator.PlayerStartSpot
        };
        for (var caveWellIndex = 1; caveWellIndex < caveWellsNumber; caveWellIndex++)
        {
            var caveWellCellIsFound =
                CellFinderLoose.TryFindRandomNotEdgeCellWith(20, validator, map, out var caveWellCell);
            if (caveWellCellIsFound)
            {
                positionsList.Add(caveWellCell);
            }
            else
            {
                CellFinderLoose.TryFindRandomNotEdgeCellWith(20, null, map, out caveWellCell);
                positionsList.Add(caveWellCell);
            }

            continue;

            bool validator(IntVec3 cell)
            {
                // Check cave well is not too close from another one.
                foreach (var otherLoc in positionsList)
                {
                    if (cell.InHorDistOf(otherLoc, DistanceBeetweenCaveWells))
                    {
                        return false;
                    }
                }

                // Check cave well is connected to map edge.
                var room = cell.GetRoom(map);
                return room is { TouchesMapEdge: true };
            }
        }

        return positionsList;
    }

    private static void SpawnAqueousCaveWellAt(Map map, IntVec3 position)
    {
        // Spawn main hole.
        SetCellsInRadiusNoRoofNoRock(map, position, 10f);
        SpawnCaveWellOpening(map, position);
        SetCellsInRadiusTerrain(map, position, 10f, TerrainDefOf.Gravel);
        SetCellsInRadiusTerrain(map, position, 8f, TerrainDefOf.WaterShallow);

        // Spawn small additional holes.
        var smallHolesNumber = Rand.RangeInclusive(2, 5);
        for (var holeIndex = 0; holeIndex < smallHolesNumber; holeIndex++)
        {
            var smallHolePosition =
                position + (7f * Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360))).ToIntVec3();
            SetCellsInRadiusNoRoofNoRock(map, smallHolePosition, 5f);
            SetCellsInRadiusTerrain(map, smallHolePosition, 3.2f, TerrainDefOf.WaterShallow);
            SetCellsInRadiusTerrain(map, smallHolePosition, 2.1f, TerrainDefOf.WaterDeep);
        }

        SetCellsInRadiusTerrain(map, position, 5.2f, TerrainDefOf.WaterDeep);
    }

    private static void SpawnCaveWellOpening(Map map, IntVec3 position)
    {
        var potentialCaveWell = position.GetFirstThing(map, Util_CaveBiome.CaveWellDef);
        if (potentialCaveWell == null)
        {
            GenSpawn.Spawn(Util_CaveBiome.CaveWellDef, position, map);
        }

        foreach (var checkedCell in GenAdjFast.AdjacentCells8Way(position))
        {
            potentialCaveWell = checkedCell.GetFirstThing(map, Util_CaveBiome.CaveWellDef);
            if (potentialCaveWell == null)
            {
                GenSpawn.Spawn(Util_CaveBiome.CaveWellDef, checkedCell, map);
            }
        }
    }

    private static void SpawnDryCaveWellWithAnimalCorpsesAt(Map map, IntVec3 position)
    {
        SpawnDryCaveWellAt(map, position);
        SpawnAnimalCorpsesMaker(map, position);
    }

    private static void SpawnDryCaveWellWithRitualStoneAt(Map map, IntVec3 position)
    {
        SpawnDryCaveWellAt(map, position);
        SpawnRitualStone(map, position);
    }

    private static void SpawnDryCaveWellAt(Map map, IntVec3 position)
    {
        // Spawn main hole.
        SetCellsInRadiusNoRoofNoRock(map, position, 10f);
        SpawnCaveWellOpening(map, position);
        SetCellsInRadiusTerrain(map, position, 10f, TerrainDefOf.Gravel);
        SetCellsInRadiusTerrain(map, position, 8f, TerrainDefOf.Soil);

        // Spawn small additional holes.
        var smallHolesNumber = Rand.RangeInclusive(2, 5);
        for (var holeIndex = 0; holeIndex < smallHolesNumber; holeIndex++)
        {
            var smallHolePosition =
                position + (7f * Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360))).ToIntVec3();
            SetCellsInRadiusNoRoofNoRock(map, smallHolePosition, 5f);
            SetCellsInRadiusTerrain(map, smallHolePosition, 3.2f, TerrainDefOf.Soil);
            SetCellsInRadiusTerrain(map, smallHolePosition, 2.1f, TerrainDef.Named("SoilRich"));
        }

        SetCellsInRadiusTerrain(map, position, 6.5f, TerrainDef.Named("SoilRich"));
    }

    private static void SpawnAnimalCorpsesMaker(Map map, IntVec3 position)
    {
        var animalCorpsesGenerator = ThingMaker.MakeThing(Util_CaveBiome.AnimalCorpsesGeneratorDef);
        GenSpawn.Spawn(animalCorpsesGenerator, position, map);
    }

    private static void SpawnRitualStone(Map map, IntVec3 position)
    {
        // Set terrain.
        SetCellsInRadiusTerrain(map, position, 2.5f, TerrainDef.Named("FlagstoneSlate"));
        // Spawn ritual stone.
        var thing = ThingMaker.MakeThing(ThingDef.Named("Sarcophagus"), ThingDef.Named("BlocksSlate"));
        GenSpawn.Spawn(thing, position + new IntVec3(0, 0, -1), map);
        (thing as Building_Sarcophagus)?.GetStoreSettings().filter.SetDisallowAll();
        // Spawn offerings.
        thing = ThingMaker.MakeThing(ThingDef.Named("MeleeWeapon_Knife"), ThingDef.Named("Jade"));
        GenSpawn.Spawn(thing, position + new IntVec3(0, 0, -1), map);
        thing = ThingMaker.MakeThing(ThingDefOf.MedicineHerbal);
        thing.stackCount = Rand.Range(5, 12);
        GenSpawn.Spawn(thing, position + new IntVec3(-1, 0, 0), map);
        thing = ThingMaker.MakeThing(ThingDefOf.Gold);
        thing.stackCount = Rand.Range(7, 25);
        GenSpawn.Spawn(thing, position + new IntVec3(1, 0, 0), map);
        thing = ThingMaker.MakeThing(ThingDef.Named("Campfire"));
        GenSpawn.Spawn(thing, position + new IntVec3(0, 0, 1), map, Rot4.South);
        // Spawn blood.
        foreach (var cell in GenRadial.RadialCellsAround(position, 2f, true))
        {
            if (cell.InBounds(map) == false)
            {
                continue;
            }

            var bloodQuantity = Rand.Range(2, 5);
            for (var bloodFilthIndex = 0; bloodFilthIndex < bloodQuantity; bloodFilthIndex++)
            {
                GenSpawn.Spawn(ThingDefOf.Filth_Blood, cell, map);
            }
        }

        // Spawn torches.
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(1, 0, 3), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(3, 0, 1), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(3, 0, -1), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(1, 0, -3), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(-1, 0, -3), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(-3, 0, -1), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(-3, 0, 1), map);
        GenSpawn.Spawn(Util_CaveBiome.CrystalLampDef, position + new IntVec3(-1, 0, 3), map);
        // Spawn corpses generator.
        if (!Rand.Bool)
        {
            return;
        }

        var villagerCorpsesGenerator = ThingMaker.MakeThing(Util_CaveBiome.VillagerCorpsesGeneratorDef);
        GenSpawn.Spawn(villagerCorpsesGenerator, position, map);
    }

    private static void SetCellsInRadiusNoRoofNoRock(Map map, IntVec3 position, float radius)
    {
        foreach (var cell in GenRadial.RadialCellsAround(position, radius, true))
        {
            if (cell.InBounds(map) == false)
            {
                continue;
            }

            // Unroof cell.
            if (cell.Roofed(map))
            {
                map.roofGrid.SetRoof(cell, null);
            }

            // Remove rock from cell.
            var rock = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(cell)];
            rock?.Destroy();

            // Remove cave roof.
            var thingList = cell.GetThingList(map).Where(thing => thing.def == Util_CaveBiome.CaveRoofDef).ToList();
            for (var i = thingList.Count - 1; i >= 0; i--)
            {
                thingList[i].Destroy();
            }
        }
    }

    private static void SetCellsInRadiusTerrain(Map map, IntVec3 position, float radius, TerrainDef terrain)
    {
        foreach (var cell in GenRadial.RadialCellsAround(position, radius, true))
        {
            if (cell.InBounds(map) == false)
            {
                continue;
            }

            if (terrain != TerrainDefOf.WaterDeep
                && terrain != TerrainDefOf.WaterOceanDeep
                && terrain != TerrainDefOf.WaterMovingChestDeep)
            {
                // Excepted when adding water, do not touch to water/marsh patches.
                var cellTerrain = map.terrainGrid.TerrainAt(cell);
                if (cellTerrain == TerrainDefOf.WaterDeep
                    || cellTerrain == TerrainDefOf.WaterOceanDeep
                    || cellTerrain == TerrainDefOf.WaterShallow
                    || cellTerrain == TerrainDefOf.WaterMovingShallow
                    || cellTerrain == TerrainDefOf.WaterOceanShallow
                    || cellTerrain == TerrainDef.Named("Marsh"))
                {
                    continue;
                }
            }

            map.terrainGrid.SetTerrain(cell, terrain);
        }
    }
}