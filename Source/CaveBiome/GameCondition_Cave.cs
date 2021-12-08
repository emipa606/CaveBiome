using RimWorld;
using UnityEngine;
using Verse;

namespace CaveBiome;

public class GameCondition_Cave : GameCondition
{
    private readonly int LerpTicks = 200;

    private SkyColorSet caveSkyColors = new SkyColorSet(new Color(0.482f, 0.603f, 0.682f), Color.white,
        new Color(0.6f, 0.6f, 0.6f), 1f);

    public override float PlantDensityFactor(Map map)
    {
        // To avoid getting plant seeds from map edges.
        return 0f;
    }

    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(TicksPassed, LerpTicks + 1f, LerpTicks);
    }

    public override SkyTarget? SkyTarget(Map map)
    {
        var weather = Util_CaveBiome.CaveCalmWeatherDef;
        caveSkyColors.sky = weather.skyColorsDay.sky;
        caveSkyColors.shadow = weather.skyColorsDay.shadow;
        caveSkyColors.saturation = weather.skyColorsDay.saturation;
        caveSkyColors.overlay = weather.skyColorsDay.overlay;
        return new SkyTarget(0f, caveSkyColors, 1f, 0f);
    }

    public override bool AllowEnjoyableOutsideNow(Map map)
    {
        return false;
    }
}