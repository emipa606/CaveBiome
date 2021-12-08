using RimWorld;
using Verse;

namespace CaveBiome;

public class IncidentWorker_PsychicEmanatorShipPartCrashInCave : IncidentWorker_ShipPartCrashInCave
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.TryExecuteWorker(parms);
        }

        return !map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicDrone) &&
               base.CanFireNowSub(parms);
    }
}