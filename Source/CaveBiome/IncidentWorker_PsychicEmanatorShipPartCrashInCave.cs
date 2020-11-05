
using Verse;
using RimWorld;

namespace CaveBiome
{
    public class IncidentWorker_PsychicEmanatorShipPartCrashInCave : IncidentWorker_ShipPartCrashInCave
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var map = (Map)parms.target;
            return !map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicDrone) && base.CanFireNowSub(parms);
        }
    }
}
