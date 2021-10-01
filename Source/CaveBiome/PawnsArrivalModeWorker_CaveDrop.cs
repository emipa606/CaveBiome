using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome
{
    // Token: 0x02000B94 RID: 2964
    public class PawnsArrivalModeWorker_CaveDrop : PawnsArrivalModeWorker
    {
        // Token: 0x06004601 RID: 17921 RVA: 0x0017944F File Offset: 0x0017764F
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            PawnsArrivalModeWorkerUtility.DropInDropPodsNearSpawnCenter(parms, pawns);
        }

        // Token: 0x06004602 RID: 17922 RVA: 0x001795D0 File Offset: 0x001777D0
        public override void TravelingTransportPodsArrived(List<ActiveDropPodInfo> dropPods, Map map)
        {
            var near = FindAGoodSpot(map);
            TransportPodsArrivalActionUtility.DropTravelingTransportPods(dropPods, near, map);
        }

        // Token: 0x06004603 RID: 17923 RVA: 0x001795F0 File Offset: 0x001777F0
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
}