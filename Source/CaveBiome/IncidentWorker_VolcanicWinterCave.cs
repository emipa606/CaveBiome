
using Verse;
using RimWorld;

namespace CaveBiome
{
    public class IncidentWorker_VolcanicWinterCave : IncidentWorker_MakeGameCondition
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var map = (Map)parms.target;
            if (map.Biome == Util_CaveBiome.CaveBiomeDef)
            {
                return false;
            }
            return base.CanFireNowSub(parms);
        }
    }
}
