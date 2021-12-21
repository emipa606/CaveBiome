using HarmonyLib;
using RimWorld;
using Verse;

namespace CaveBiome;

[HarmonyPatch(typeof(ShortCircuitUtility), "TryShortCircuitInRain", typeof(Thing))]
public class ShortCircuitUtility_TryShortCircuitInRain
{
    private static bool Prefix(Thing thing, ref bool __result)
    {
        if (thing.Map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return true;
        }

        var caveWellsList = thing.Map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        if (caveWellsList.Any(caveWell => caveWell.Position == thing.Position))
        {
            return true;
        }

        __result = false;
        return false;
    }
}