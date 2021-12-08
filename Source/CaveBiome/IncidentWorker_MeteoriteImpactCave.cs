using RimWorld;
using Verse;

namespace CaveBiome;

public class IncidentWorker_MeteoriteImpactCave : IncidentWorker_MeteoriteImpact
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.CanFireNowSub(parms);
        }

        return false;
    }
}