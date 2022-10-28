using RimWorld;
using Verse;

namespace CaveBiome;

public class IncidentWorker_VolcanicWinterCave : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var map = (Map)parms.target;
        return map.Biome != Util_CaveBiome.CaveBiomeDef && base.CanFireNowSub(parms);
    }
}