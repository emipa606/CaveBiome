using RimWorld;
using UnityEngine;
using Verse;

namespace CaveBiome;

public class IncidentWorker_ShipChunkDropInCave : IncidentWorker_ShipChunkDrop
{
    private static readonly Pair<int, float>[] CountChance =
    {
        new Pair<int, float>(1, 1f),
        new Pair<int, float>(2, 0.95f),
        new Pair<int, float>(3, 0.7f),
        new Pair<int, float>(4, 0.4f)
    };

    private int RandomCountToDrop
    {
        get
        {
            var x2 = Find.TickManager.TicksGame / 3600000f;
            var timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
            return CountChance.RandomElementByWeight(delegate(Pair<int, float> x)
            {
                if (x.First == 1)
                {
                    return x.Second;
                }

                return x.Second * timePassedFactor;
            }).First;
        }
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.TryExecuteWorker(parms);
        }

        TryFindShipChunkDropSpot(map, out var firstChunkPosition);
        if (!firstChunkPosition.IsValid)
        {
            return false;
        }

        // Spawn ship chunks.
        var partsCount = RandomCountToDrop;
        GenSpawn.Spawn(ThingDefOf.ShipChunk, firstChunkPosition, map);
        for (var shipShunkIndex = 0; shipShunkIndex < partsCount - 1; shipShunkIndex++)
        {
            TryFindShipChunkDropSpotNear(map, firstChunkPosition, out var nexChunkPosition);
            if (nexChunkPosition.IsValid)
            {
                GenSpawn.Spawn(ThingDefOf.ShipChunk, nexChunkPosition, map);
            }
        }

        Messages.Message("MessageShipChunkDrop".Translate(), new TargetInfo(firstChunkPosition, map),
            MessageTypeDefOf.NeutralEvent);
        return true;
    }

    private void TryFindShipChunkDropSpot(Map map, out IntVec3 spawnCell)
    {
        spawnCell = IntVec3.Invalid;
        var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        foreach (var caveWell in caveWellsList.InRandomOrder())
        {
            if (!IsValidPositionToSpawnShipChunk(map, caveWell.Position))
            {
                continue;
            }

            spawnCell = caveWell.Position;
            return;
        }
    }

    private void TryFindShipChunkDropSpotNear(Map map, IntVec3 root, out IntVec3 spawnCell)
    {
        spawnCell = IntVec3.Invalid;
        foreach (var checkedPosition in GenRadial.RadialCellsAround(root, 5f, false))
        {
            if (!IsValidPositionToSpawnShipChunk(map, checkedPosition))
            {
                continue;
            }

            spawnCell = checkedPosition;
            return;
        }
    }

    private bool IsValidPositionToSpawnShipChunk(Map map, IntVec3 position)
    {
        var chunkDef = ThingDefOf.ShipChunk;
        if (position.InBounds(map) == false
            || position.Fogged(map)
            || position.Standable(map) == false
            || position.Roofed(map)
            && position.GetRoof(map).isThickRoof)
        {
            return false;
        }

        if (position.SupportsStructureType(map, chunkDef.terrainAffordanceNeeded) == false)
        {
            return false;
        }

        var thingList = position.GetThingList(map);
        foreach (var thing in thingList)
        {
            if (thing.def.category != ThingCategory.Plant
                && GenSpawn.SpawningWipes(chunkDef, thing.def))
            {
                return false;
            }
        }

        return true;
    }
}