using System.Linq;
using RimWorld;
using Verse;

namespace CaveBiome;

public abstract class IncidentWorker_ShipPartCrashInCave : IncidentWorker_CrashedShipPart
{
    private const float ShipPointsFactor = 0.9f;

    protected virtual int CountToSpawn => 1;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.CanFireNowSub(parms);
        }

        return map.listerThings.ThingsOfDef(def.mechClusterBuilding).Count <= 0;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.TryExecuteWorker(parms);
        }

        var num = 0;
        var countToSpawn = CountToSpawn;
        var vec = IntVec3.Invalid;
        for (var i = 0; i < countToSpawn; i++)
        {
            TryFindShipCrashSite(map, out var spawnCell);

            if (spawnCell.IsValid == false)
            {
                break;
            }

            GenExplosion.DoExplosion(spawnCell, map, 3f, DamageDefOf.Flame, null);
            var building_CrashedShipPart = (Building)GenSpawn.Spawn(def.mechClusterBuilding, spawnCell, map);
            building_CrashedShipPart.SetFaction(Faction.OfMechanoids);
            var points = parms.points * ShipPointsFactor;
            _ = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = Faction.OfMechanoids,
                points = points
            }).ToList();
            num++;
            vec = spawnCell;
        }

        if (num <= 0)
        {
            return false;
        }

        Find.CameraDriver.shaker.DoShake(1f);
        Find.LetterStack.ReceiveLetter(def.letterLabel, def.letterText, def.letterDef,
            new TargetInfo(vec, map));

        return true;
    }

    private void TryFindShipCrashSite(Map map, out IntVec3 spawnCell)
    {
        spawnCell = IntVec3.Invalid;
        var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        foreach (var caveWell in caveWellsList.InRandomOrder())
        {
            if (!IsValidPositionForShipCrashSite(map, caveWell.Position))
            {
                continue;
            }

            spawnCell = caveWell.Position;
            return;
        }
    }

    private bool IsValidPositionForShipCrashSite(Map map, IntVec3 position)
    {
        if (position.InBounds(map) == false
            || position.Fogged(map))
        {
            return false;
        }

        foreach (var checkedPosition in GenAdj.CellsOccupiedBy(position, Rot4.North, def.mechClusterBuilding.size))
        {
            if (checkedPosition.Standable(map) == false
                || checkedPosition.Roofed(map)
                || map.reachability.CanReachColony(checkedPosition) == false)
            {
                return false;
            }
        }

        return true;
    }
}