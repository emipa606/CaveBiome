using System;
using System.Collections.Generic;

using UnityEngine;
using Verse;
using Verse.Noise;
using RimWorld;

namespace CaveBiome
{
    public class GenStep_CaveRiver : GenStep
    {
        public const int OutOfBoundsOffset = 5;
        public const float RiverPartsGranularity = 5f;
        public const float RiverLateralFactor = 30f;
        public const float RiverWidthFactor = 10f;
        public override int SeedPart => 647813558;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                // Nothing to do.
                return;
            }
            
            // Get river origin side.
            var riverEntrySideAsInt = Rand.RangeInclusive(2, 3);
            var riverEntrySide = new Rot4(riverEntrySideAsInt);

            // Get river origin and end coordinates.
            Vector2 riverStart;
            Vector2 riverEnd;
            switch (riverEntrySideAsInt)
            {
                default:
                case 2: // South.
                    if ((riverEntrySideAsInt == 0)
                        || (riverEntrySideAsInt == 1))
                    {
                        Log.Warning("CaveBiome: river entry side (" + riverEntrySideAsInt + ") should not occur.");
                    }
                    riverStart = new Vector2(Rand.RangeInclusive((int)(map.Size.x * 0.25f), (int)(map.Size.x * 0.75f)), -OutOfBoundsOffset);
                    riverEnd = new Vector2(Rand.RangeInclusive((int)(map.Size.x * 0.25f), (int)(map.Size.x * 0.75f)), map.Size.z + OutOfBoundsOffset);
                    break;
                case 3: // West.
                    riverStart = new Vector2(-OutOfBoundsOffset, Rand.RangeInclusive((int)(map.Size.z * 0.25f), (int)(map.Size.z * 0.75f)));
                    riverEnd = new Vector2(map.Size.x + OutOfBoundsOffset, Rand.RangeInclusive((int)(map.Size.z * 0.25f), (int)(map.Size.z * 0.75f)));
                    break;
            }

            // Get straight river points.
            Vector2 riverVector = riverEnd - riverStart;
            var numberOfParts = (int)Math.Ceiling(riverVector.magnitude / RiverPartsGranularity);
            Vector2 riverVectorNormalized = riverVector / numberOfParts;
            var riverCoordinates = new List<Vector2>();
            Vector2 vector = riverStart;
            for (var partIndex = 0; partIndex < numberOfParts; partIndex++)
            {
                riverCoordinates.Add(vector);
                vector += riverVectorNormalized;
            }
            
            // Generate Perlin map and apply perturbations.
            var perlinMap = new Perlin(0.05, 2.0, 0.5, 4, Rand.Range(0, 2147483647), QualityMode.High);
            List<Vector2> riverCoordinatesCopy = riverCoordinates.ListFullCopy<Vector2>();
            riverCoordinates.Clear();
            var riverWidth = new List<float>();
            for (var coordinatesIndex = 0; coordinatesIndex < riverCoordinatesCopy.Count; coordinatesIndex++)
            {
                var perturbation = RiverLateralFactor * (float)perlinMap.GetValue(coordinatesIndex, 0.0, 0.0);
                Vector2 pointCoordinates;
                if (riverEntrySide == Rot4.South)
                {
                    pointCoordinates = riverCoordinatesCopy[coordinatesIndex] + (perturbation * Vector2.right);
                }
                else
                {
                    pointCoordinates = riverCoordinatesCopy[coordinatesIndex] + (perturbation * Vector2.down);
                }
                riverCoordinates.Add(pointCoordinates);

                var width = Mathf.Min(Mathf.Abs(RiverWidthFactor * (float)perlinMap.GetValue(0.0, 0.0, coordinatesIndex)), 8f);
                riverWidth.Add(width);
            }

            // Generate river from coordinates.
            for (var coordinatesIndex = 0; coordinatesIndex < riverCoordinates.Count - 1; coordinatesIndex++)
            {
                var width = Mathf.Max(2.5f, riverWidth[coordinatesIndex]);
                Vector2 riverPart = riverCoordinates[coordinatesIndex + 1] - riverCoordinates[coordinatesIndex];
                Vector2 riverPartNormalized = riverPart / RiverPartsGranularity;
                for (var pointIndex = 0; pointIndex < RiverPartsGranularity; pointIndex++)
                {
                    IntVec3 point = new IntVec3(riverCoordinates[coordinatesIndex]) + new IntVec3(pointIndex * riverPartNormalized);
                    // Generate mud.
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(point, width + 1.9f, true))
                    {
                        if (cell.InBounds(map) == false)
                        {
                            continue;
                        }
                        Thing building = cell.GetEdifice(map);
                        if (building != null)
                        {
                            continue;
                        }
                        TerrainDef terrain = map.terrainGrid.TerrainAt(cell);
                        // Do not change stony terrains and do not put mud on top of existing water patch.
                        if ((terrain.defName.Contains("Rough") == false)
                            && (terrain != TerrainDefOf.WaterShallow)
                            && (terrain != TerrainDefOf.WaterDeep)
                            && (terrain != TerrainDefOf.WaterOceanShallow)
                            && (terrain != TerrainDefOf.WaterOceanDeep)
                            && (terrain != TerrainDefOf.WaterMovingShallow)
                            && (terrain != TerrainDefOf.WaterMovingChestDeep))
                        {
                            map.terrainGrid.SetTerrain(cell, TerrainDef.Named("Mud"));
                        }
                    }
                    // Generate shallow moving water and remove building.
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(point, width, true))
                    {
                        if (cell.InBounds(map) == false)
                        {
                            continue;
                        }
                        Thing building = cell.GetEdifice(map);
                        if (building != null)
                        {
                            building.Destroy();
                        }
                        TerrainDef terrain = map.terrainGrid.TerrainAt(cell);
                        if ((terrain != TerrainDefOf.WaterDeep)
                            && (terrain != TerrainDefOf.WaterMovingChestDeep)
                            &&( terrain != TerrainDefOf.WaterOceanDeep))
                        {
                            map.terrainGrid.SetTerrain(cell, TerrainDefOf.WaterMovingShallow);
                        }
                    }
                    // Generate deep water.
                    if (width > 4)
                    {
                        foreach (IntVec3 cell in GenRadial.RadialCellsAround(point, width - 3.9f, true))
                        {
                            if (cell.InBounds(map) == false)
                            {
                                continue;
                            }
                            map.terrainGrid.SetTerrain(cell, TerrainDefOf.WaterMovingChestDeep);
                        }
                    }
                }
            }
            IntVec3 offset = riverEntrySide == Rot4.South ? new IntVec3(8, 0, 0) : new IntVec3(0, 0, 8);
            // Set player start spot.
            MapGenerator.PlayerStartSpot = new IntVec3(riverCoordinates[riverCoordinates.Count / 2]) + offset;
        }
    }
}
