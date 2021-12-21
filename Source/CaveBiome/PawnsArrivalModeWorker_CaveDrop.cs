using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome;

public class PawnsArrivalModeWorker_CaveDrop : PawnsArrivalModeWorker
{
    public override void Arrive(List<Pawn> pawns, IncidentParms parms)
    {
        PawnsArrivalModeWorkerUtility.DropInDropPodsNearSpawnCenter(parms, pawns);
    }

    public override void TravelingTransportPodsArrived(List<ActiveDropPodInfo> dropPods, Map map)
    {
        var near = FindAGoodSpot(map);
        TransportPodsArrivalActionUtility.DropTravelingTransportPods(dropPods, near, map);
    }

    public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
    {
        var map = (Map)parms.target;
        parms.spawnCenter = FindAGoodSpot(map);
        if (parms.spawnCenter == IntVec3.Invalid)
        {
            return false;
        }

        parms.spawnRotation = Rot4.Random;
        return true;
    }

    private IntVec3 FindAGoodSpot(Map map)
    {
        var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        if (caveWellsList.Count == 0)
        {
            return IntVec3.Invalid;
        }

        if ((from caveWell in caveWellsList
                where DropCellFinder.TryFindDropSpotNear(caveWell.Position, map, out _, false, false)
                select caveWell.Position).TryRandomElement(out var dropPlace))
        {
            return dropPlace;
        }

        return IntVec3.Invalid;
    }
}