using System.Reflection;
using DeepRim;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DeepRimPatch
{
    [StaticConstructorOnStartup]
    internal static class DeepRimPatch
    {
        static DeepRimPatch()
        {
            var harmony = new Harmony("Mlie.CaveBiome.DeepRimPatch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Map))]
        [HarmonyPatch("Biome")]
        [HarmonyAfter("com.deeprim.rimworld.mod")]
        private static void MapBiomePostfix(Map __instance, ref BiomeDef __result)
        {
            if (__instance.ParentHolder is UndergroundMapParent)
            {
                __result = DefDatabase<BiomeDef>.GetNamed("Cave");
            }
        }
    }
}