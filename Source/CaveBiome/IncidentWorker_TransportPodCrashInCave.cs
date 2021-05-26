using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome
{
    public class IncidentWorker_TransportPodCrashInCave : IncidentWorker_TransportPodCrash
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                return TryExecute(parms);
            }

            TryFindRefugeePodSpot(map, out var intVec);
            if (intVec.IsValid == false)
            {
                return false;
            }

            var faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.OutlanderRefugee);
            var request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, faction,
                PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 20f, false, true, true,
                false);
            var pawn = PawnGenerator.GeneratePawn(request);
            HealthUtility.DamageUntilDowned(pawn);
            var label = "LetterLabelRefugeePodCrash".Translate();
            var text = "RefugeePodCrash".Translate();
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, new GlobalTargetInfo(intVec, map));
            DropPodUtility.MakeDropPodAt(intVec, map, new ActiveDropPodInfo
            {
                SingleContainedThing = pawn,
                openDelay = 180,
                leaveSlag = true
            });
            return true;
        }

        private static void TryFindRefugeePodSpot(Map map, out IntVec3 spawnCell)
        {
            spawnCell = IntVec3.Invalid;
            var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
            foreach (var caveWell in caveWellsList.InRandomOrder())
            {
                if (!IsValidPositionToSpawnRefugeePod(map, caveWell.Position))
                {
                    continue;
                }

                spawnCell = caveWell.Position;
                return;
            }
        }

        private static bool IsValidPositionToSpawnRefugeePod(Map map, IntVec3 position)
        {
            if (position.InBounds(map) == false
                || position.Fogged(map)
                || position.Standable(map) == false
                || position.Roofed(map)
                && position.GetRoof(map).isThickRoof)
            {
                return false;
            }

            return true;
        }
    }
}