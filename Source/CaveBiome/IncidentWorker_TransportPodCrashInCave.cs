using System.Collections.Generic;

using Verse;
using RimWorld;
using RimWorld.Planet;

namespace CaveBiome
{
    public class IncidentWorker_TransportPodCrashInCave : IncidentWorker_TransportPodCrash
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map)parms.target;
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                return TryExecute(parms);
            }
            TryFindRefugeePodSpot(map, out IntVec3 intVec);
            if (intVec.IsValid == false)
            {
                return false;
            }
            Faction faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.OutlanderRefugee);
            var request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 20f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            HealthUtility.DamageUntilDowned(pawn);
            TaggedString label = "LetterLabelRefugeePodCrash".Translate();
            TaggedString text = "RefugeePodCrash".Translate();
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, new GlobalTargetInfo(intVec, map, false), null);
            DropPodUtility.MakeDropPodAt(intVec, map, new ActiveDropPodInfo
            {
                SingleContainedThing = pawn,
                openDelay = 180,
                leaveSlag = true
            });
            return true;
        }

        public static void TryFindRefugeePodSpot(Map map, out IntVec3 spawnCell)
        {
            spawnCell = IntVec3.Invalid;
            List<Thing> caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
            foreach (Thing caveWell in caveWellsList.InRandomOrder())
            {
                if (IsValidPositionToSpawnRefugeePod(map, caveWell.Position))
                {
                    spawnCell = caveWell.Position;
                    return;
                }
            }
        }

        public static bool IsValidPositionToSpawnRefugeePod(Map map, IntVec3 position)
        {
            if ((position.InBounds(map) == false)
                || position.Fogged(map)
                || (position.Standable(map) == false)
                || (position.Roofed(map)
                    && position.GetRoof(map).isThickRoof))
            {
                return false;
            }
            return true;
        }
    }
}
