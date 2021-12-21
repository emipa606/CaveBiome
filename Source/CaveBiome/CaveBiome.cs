using System.Reflection;
using HarmonyLib;
using Verse;

namespace CaveBiome;

[StaticConstructorOnStartup]
public static class CaveBiome
{
    static CaveBiome()
    {
        var harmony = new Harmony("Mlie.CaveBiome.ShortCircutPatch");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}