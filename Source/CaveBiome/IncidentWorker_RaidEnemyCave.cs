using RimWorld;
using Verse;

namespace CaveBiome
{
    public class IncidentWorker_RaidEnemyCave : IncidentWorker_RaidEnemy
    {
        public override void ResolveRaidArriveMode(IncidentParms parms)
        {
            var map = (Map)parms.target;
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                base.ResolveRaidArriveMode(parms);
                return;
            }

            parms.raidArrivalMode = parms.faction.def.techLevel >= TechLevel.Industrial
                ? DefDatabase<PawnsArrivalModeDef>.GetNamedSilentFail("CaveDrop")
                : PawnsArrivalModeDefOf.EdgeWalkIn;
        }

        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            var map = (Map)parms.target;
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                base.ResolveRaidStrategy(parms, groupKind);
                return;
            }

            if (Rand.Bool)
            {
                parms.raidStrategy = Rand.Bool
                    ? RaidStrategyDefOf.ImmediateAttack
                    : DefDatabase<RaidStrategyDef>.GetNamedSilentFail("ImmediateAttackSmart");
            }
            else
            {
                parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamedSilentFail("StageThenAttack");
            }
        }
    }
}