using System.Reflection;
using HarmonyLib;
using Verse;

namespace CaveBiome;

[StaticConstructorOnStartup]
public static class CaveBiome
{
    static CaveBiome()
    {
        new Harmony("Mlie.CaveBiome.ShortCircutPatch").PatchAll(Assembly.GetExecutingAssembly());
    }
}