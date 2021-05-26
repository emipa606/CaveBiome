using RimWorld;
using Verse;

namespace CaveBiome
{
    public class IncidentWorker_FlashstormCave : IncidentWorker_Flashstorm
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var map = (Map) parms.target;
            if (map.Biome == Util_CaveBiome.CaveBiomeDef)
            {
                return false;
            }

            return base.CanFireNowSub(parms);
        }
    }
}