using System;
using System.Collections.Generic;
//using VerseBase;   // Material/Graphics handling functions are found here
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.AI;    // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace CaveBiome
{
    /// <summary>
    /// MapComponent_CaveWellLight class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class MapComponent_CaveWellLight : MapComponent
    {
        public const int sunriseBeginHour = 6;
        public const int sunriseEndHour = 10;
        public const int sunsetBeginHour = 16;
        public const int sunsetEndHour = 20;
        public const int lightCheckPeriodInTicks = GenTicks.TicksPerRealSecond;
        public int nextLightCheckTick;

        public int gamehourDebugMessage;

        public const float brightnessCaveWellMin = 0f;
        public const float brightnessCaveWellMax = 1f;

        public static bool plantsMessageHasBeenSent;
        public static bool growingMessageHasBeenSent;

        public static float glowRadiusCaveWellDay;

        public const float lightRadiusCaveWellMax = 10f;

        public static ColorInt baseGlowColor;
        public static ColorInt currentGlowColor;

        public MapComponent_CaveWellLight(Map map) : base(map)
        {
            nextLightCheckTick = 1;
            gamehourDebugMessage = 0;
            glowRadiusCaveWellDay = 10f;
            baseGlowColor = new ColorInt(370, 370, 370);
            currentGlowColor = new ColorInt(0, 0, 0);

            //InstantiateGlow();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref plantsMessageHasBeenSent, "plantsMessageHasBeenSent");
            Scribe_Values.Look<bool>(ref growingMessageHasBeenSent, "growingMessageHasBeenSent");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                MapComponentTick();
            }
        }

        public override void MapComponentTick()
        {
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                return;
            }

            if (Find.TickManager.TicksGame >= nextLightCheckTick)
            {
                nextLightCheckTick = Find.TickManager.TicksGame + lightCheckPeriodInTicks;
                var gamehour = GenDate.HoursPerDay * GenDate.DayPercent(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x); // TODO: could refine to accommodate axial tilt, such that high latitudes will have "midnight sun" growing areas... nifty.
                float caveWellBrightness;
                if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
                {
                    // Shut down light when there is an eclipse.
                    caveWellBrightness = 0.0f;
                }
                else
                {
                    if (gamehour < sunriseBeginHour)
                    {
                        caveWellBrightness = brightnessCaveWellMin;
                    }
                    else if (gamehour < sunriseEndHour)
                    {
                        var sunriseProgress = Math.Max(0f, gamehour - sunriseBeginHour) / (sunriseEndHour - sunriseBeginHour);
                        caveWellBrightness = sunriseProgress * brightnessCaveWellMax;
                    }
                    else if (gamehour < sunsetBeginHour)
                    {
                        caveWellBrightness = brightnessCaveWellMax;
                    }
                    else if (gamehour < sunsetEndHour)
                    {
                        var sunsetProgress = Math.Max(0f, gamehour - sunsetBeginHour) / (sunsetEndHour - sunsetBeginHour);
                        caveWellBrightness = 1 - (sunsetProgress * brightnessCaveWellMax);
                    }
                    else
                    {
                        caveWellBrightness = brightnessCaveWellMin;
                    }
                }

                currentGlowColor.r = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.r);
                currentGlowColor.g = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.g);
                currentGlowColor.b = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.b);

                List<Thing> caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
                foreach (Thing caveWell in caveWellsList)
                {
                    SetCaveWellBrightness(caveWell, caveWellBrightness);
                }

                if ((plantsMessageHasBeenSent == false) && (gamehour >= sunriseBeginHour + 1))
                {
                    Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelCavePlants".Translate(), "CaveBiome.CavePlants".Translate(),
                        LetterDefOf.PositiveEvent);
                    plantsMessageHasBeenSent = true;
                }
                if ((growingMessageHasBeenSent == false) && (gamehour >= sunriseBeginHour + 2))
                {
                    if (MapGenerator.PlayerStartSpot.IsValid && (MapGenerator.PlayerStartSpot != IntVec3.Zero)) // Checking PlayerStartSpot validity will still raise an error message if it is invalid.
                    {
                        Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelGrowingInCave".Translate(), "CaveBiome.GrowingInCave".Translate(), LetterDefOf.PositiveEvent, new RimWorld.Planet.GlobalTargetInfo(MapGenerator.PlayerStartSpot, map));
                    }
                    else
                    {
                        Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelGrowingInCave".Translate(), "CaveBiome.GrowingInCave".Translate(), LetterDefOf.PositiveEvent);
                    }
                    growingMessageHasBeenSent = true;
                }
            }
        }

        public void InstantiateGlow()
        {
            CompProperties compProps = DefDatabase<ThingDef>.GetNamed("CaveWell").CompDefFor<CompGlower>();
            if (compProps is CompProperties_Glower glowerCompProps)
            {
                baseGlowColor.r = glowerCompProps.glowColor.r;
                baseGlowColor.g = glowerCompProps.glowColor.g;
                baseGlowColor.b = glowerCompProps.glowColor.b;

                glowRadiusCaveWellDay = glowerCompProps.glowRadius;
            }
        }

        public void SetCaveWellBrightness(Thing caveWell, float intensity)
        {
            CompGlower glowerComp = caveWell.TryGetComp<CompGlower>();
            if (glowerComp is CompGlower)
            {
                glowerComp.Props.glowRadius = intensity * lightRadiusCaveWellMax;
                glowerComp.Props.overlightRadius = intensity * lightRadiusCaveWellMax;
                glowerComp.Props.glowColor = currentGlowColor;
                caveWell.Map.glowGrid.MarkGlowGridDirty(caveWell.Position);
            }
        }
    }
}
