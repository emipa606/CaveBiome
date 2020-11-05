
using Verse;
using RimWorld;
using UnityEngine;

namespace CaveBiome
{
    public class GameCondition_Cave : GameCondition
    {
        private readonly int LerpTicks = 200;
        
        public override float PlantDensityFactor(Map map)
        {
            // To avoid getting plant seeds from map edges.
            return 0f;
        }

		private SkyColorSet caveSkyColors = new SkyColorSet(new Color(0.482f, 0.603f, 0.682f), Color.white, new Color(0.6f, 0.6f, 0.6f), 1f);

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(TicksPassed, LerpTicks + 1f, LerpTicks, 1f);
        }
        public override SkyTarget? SkyTarget(Map map)
        {
			WeatherDef weather = Util_CaveBiome.CaveCalmWeatherDef;
			caveSkyColors.sky = weather.skyColorsDay.sky;
			caveSkyColors.shadow = weather.skyColorsDay.shadow;
			caveSkyColors.saturation = weather.skyColorsDay.saturation;
			caveSkyColors.overlay = weather.skyColorsDay.overlay;
            return new SkyTarget?(new SkyTarget(0f, caveSkyColors, 1f, 0f));
        }
		
		public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }
    }
}
