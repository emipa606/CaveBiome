using RimWorld;
using Verse;

namespace CaveBiome;

public class GenStep_CaveInitializeWeatherAndLight : GenStep
{
    public override int SeedPart => 647313558;

    public override void Generate(Map map, GenStepParams parms)
    {
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            // Nothing to do in other biomes.
            return;
        }

        // To avoid starting with standard Clear weather, immediately force to reselect a cave biome weather.
        map.weatherDecider.StartNextWeather();

        var condition = GameConditionMaker.MakeConditionPermanent(Util_CaveBiome.CaveEnvironmentGameConditionDef);
        map.gameConditionManager.RegisterCondition(condition);
    }
}