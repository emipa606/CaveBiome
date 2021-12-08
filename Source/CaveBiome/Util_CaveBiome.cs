using RimWorld;
using Verse;

namespace CaveBiome;

public static class Util_CaveBiome
{
    // Crystal lamp.
    public static ThingDef CrystalLampDef => ThingDef.Named("CrystalLamp");

    // Roof and cave well.
    public static ThingDef CaveRoofDef => ThingDef.Named("CaveRoof");

    public static ThingDef CaveWellDef => ThingDef.Named("CaveWell");

    // Weather and light.
    public static WeatherDef CaveCalmWeatherDef => WeatherDef.Named("CaveCalm");

    public static GameConditionDef CaveEnvironmentGameConditionDef => GameConditionDef.Named("CaveEnvironment");

    // Biome.
    public static BiomeDef CaveBiomeDef => BiomeDef.Named("Cave");

    // Corpses generators.
    public static ThingDef AnimalCorpsesGeneratorDef => ThingDef.Named("AnimalCorpsesGenerator");

    public static ThingDef VillagerCorpsesGeneratorDef => ThingDef.Named("VillagerCorpsesGenerator");
}