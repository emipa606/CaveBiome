using RimWorld;
using Verse;

namespace CaveBiome;

public class IncidentWorker_ResourcePodCrashInCave : IncidentWorker_ResourcePodCrash
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (map.Biome != Util_CaveBiome.CaveBiomeDef)
        {
            return base.TryExecuteWorker(parms);
        }

        var things = ThingSetMakerDefOf.ResourcePod.root.Generate();
        IntVec3 intVec;
        if (map.Biome == Util_CaveBiome.CaveBiomeDef)
        {
            tryFindDropPodSpot(map, out intVec);
            if (!intVec.IsValid)
            {
                return false;
            }
        }
        else
        {
            intVec = DropCellFinder.RandomDropSpot(map);
        }

        DropPodUtility.DropThingsNear(intVec, map, things, 110, false, true);
        SendStandardLetter("LetterLabelCargoPodCrash".Translate(), "CargoPodCrash".Translate(),
            LetterDefOf.PositiveEvent, parms, new TargetInfo(intVec, map));
        return true;
    }

    private static void tryFindDropPodSpot(Map map, out IntVec3 spawnCell)
    {
        spawnCell = IntVec3.Invalid;
        var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
        foreach (var caveWell in caveWellsList.InRandomOrder())
        {
            if (!isValidPositionToSpawnDropPod(map, caveWell.Position))
            {
                continue;
            }

            spawnCell = caveWell.Position;
            return;
        }
    }

    private static bool isValidPositionToSpawnDropPod(Map map, IntVec3 position)
    {
        return position.InBounds(map)
               && !position.Fogged(map)
               && position.Standable(map)
               && (!position.Roofed(map)
                   || !position.GetRoof(map).isThickRoof);
    }
}