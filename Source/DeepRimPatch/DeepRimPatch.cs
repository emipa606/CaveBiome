using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace DeepRimPatch
{

    [StaticConstructorOnStartup]
    static class DeepRimPatch
    {
        static DeepRimPatch()
        {
            var harmony = new Harmony("Mlie.CaveBiome.DeepRimPatch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Map))]
        [HarmonyPatch("Biome")]
        [HarmonyAfter(new string[] { "com.deeprim.rimworld.mod" })]
        private static void MapBiomePostfix(Map __instance, ref BiomeDef __result)
        {
            bool flag = __instance.ParentHolder is DeepRim.UndergroundMapParent;
            if (flag)
            {
                __result = DefDatabase<BiomeDef>.GetNamed("Cave", true);
            }
        }
    }
}
