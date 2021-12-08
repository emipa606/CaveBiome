using Verse;

namespace CaveBiome;

public class PlaceWorker_NotInCave : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        if (map.Biome == Util_CaveBiome.CaveBiomeDef)
        {
            return new AcceptanceReport("CaveBiome.CanNotBuildInCave".Translate());
        }

        return true;
    }
}