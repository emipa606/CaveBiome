using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome;

/// <summary>
///     MapComponent_CaveWellLight class.
/// </summary>
/// <author>Rikiki</author>
/// <permission>
///     Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
///     Remember learning is always better than just copy/paste...
/// </permission>
public class MapComponent_CaveWellLight : MapComponent
{
    private const int sunriseBeginHour = 6;
    private const int sunriseEndHour = 10;
    private const int sunsetBeginHour = 16;
    private const int sunsetEndHour = 20;
    private const int lightCheckPeriodInTicks = GenTicks.TicksPerRealSecond;

    private const float brightnessCaveWellMin = 0f;
    private const float brightnessCaveWellMax = 1f;

    public const float lightRadiusCaveWellMin = 0f;
    private const float lightRadiusCaveWellMax = 10f;

    private static bool plantsMessageHasBeenSent;
    private static bool growingMessageHasBeenSent;

    private static float glowRadiusCaveWellDay;
    public static float glowRadiusCaveWellNight = 0f;

    private static ColorInt baseGlowColor;
    private static ColorInt currentGlowColor;

    private int gamehourDebugMessage;
    private int nextLightCheckTick;

    public MapComponent_CaveWellLight(Map map) : base(map)
    {
        InstantiateGlow();
        nextLightCheckTick = 1;
        gamehourDebugMessage = 0;
        glowRadiusCaveWellDay = 10f;
        baseGlowColor = new ColorInt(370, 370, 370);
        currentGlowColor = new ColorInt(0, 0, 0);
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref plantsMessageHasBeenSent, "plantsMessageHasBeenSent");
        Scribe_Values.Look(ref growingMessageHasBeenSent, "growingMessageHasBeenSent");
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

        if (Find.TickManager.TicksGame < nextLightCheckTick)
        {
            return;
        }

        nextLightCheckTick = Find.TickManager.TicksGame + lightCheckPeriodInTicks;
        var gamehour = GenDate.HoursPerDay *
                       GenDate.DayPercent(Find.TickManager.TicksAbs,
                           Find.WorldGrid.LongLatOf(map.Tile)
                               .x); // TODO: could refine to accommodate axial tilt, such that high latitudes will have "midnight sun" growing areas... nifty.
        float caveWellBrightness;
        if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
        {
            // Shut down light when there is an eclipse.
            caveWellBrightness = 0.0f;
        }
        else
        {
            switch (gamehour)
            {
                case < sunriseBeginHour:
                    caveWellBrightness = brightnessCaveWellMin;
                    break;
                case < sunriseEndHour:
                {
                    var sunriseProgress = Math.Max(0f, gamehour - sunriseBeginHour) /
                                          (sunriseEndHour - sunriseBeginHour);
                    caveWellBrightness = sunriseProgress * brightnessCaveWellMax;
                    break;
                }
                case < sunsetBeginHour:
                    caveWellBrightness = brightnessCaveWellMax;
                    break;
                case < sunsetEndHour:
                {
                    var sunsetProgress =
                        Math.Max(0f, gamehour - sunsetBeginHour) / (sunsetEndHour - sunsetBeginHour);
                    caveWellBrightness = 1 - (sunsetProgress * brightnessCaveWellMax);
                    break;
                }
                default:
                    caveWellBrightness = brightnessCaveWellMin;
                    break;
            }
        }

        currentGlowColor.r = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.r);
        currentGlowColor.g = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.g);
        currentGlowColor.b = (int)(caveWellBrightness * caveWellBrightness * baseGlowColor.b);

        var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        foreach (var caveWell in caveWellsList)
        {
            SetCaveWellBrightness(caveWell, caveWellBrightness);
        }

        if (plantsMessageHasBeenSent == false && gamehour >= sunriseBeginHour + 1)
        {
            Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelCavePlants".Translate(),
                "CaveBiome.CavePlants".Translate(),
                LetterDefOf.PositiveEvent);
            plantsMessageHasBeenSent = true;
        }

        if (growingMessageHasBeenSent || !(gamehour >= sunriseBeginHour + 2))
        {
            return;
        }

        if (MapGenerator.PlayerStartSpot.IsValid && MapGenerator.PlayerStartSpot != IntVec3.Zero
           ) // Checking PlayerStartSpot validity will still raise an error message if it is invalid.
        {
            Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelGrowingInCave".Translate(),
                "CaveBiome.GrowingInCave".Translate(), LetterDefOf.PositiveEvent,
                new GlobalTargetInfo(MapGenerator.PlayerStartSpot, map));
        }
        else
        {
            Find.LetterStack.ReceiveLetter("CaveBiome.LetterLabelGrowingInCave".Translate(),
                "CaveBiome.GrowingInCave".Translate(), LetterDefOf.PositiveEvent);
        }

        growingMessageHasBeenSent = true;
    }

    private void InstantiateGlow()
    {
        var compProps = DefDatabase<ThingDef>.GetNamed("CaveWell").CompDefFor<CompGlower>();
        if (compProps is not CompProperties_Glower glowerCompProps)
        {
            return;
        }

        baseGlowColor.r = glowerCompProps.glowColor.r;
        baseGlowColor.g = glowerCompProps.glowColor.g;
        baseGlowColor.b = glowerCompProps.glowColor.b;

        glowRadiusCaveWellDay = glowerCompProps.glowRadius;
    }

    private void SetCaveWellBrightness(Thing caveWell, float intensity)
    {
        var glowerComp = caveWell.TryGetComp<CompGlower>();
        if (glowerComp is null)
        {
            return;
        }

        glowerComp.Props.glowRadius = intensity * lightRadiusCaveWellMax;
        glowerComp.Props.overlightRadius = intensity * lightRadiusCaveWellMax;
        glowerComp.Props.glowColor = currentGlowColor;
        caveWell.Map.glowGrid.DirtyCell(caveWell.Position);
    }
}