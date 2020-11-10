
using Verse;
using RimWorld;

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
            if (parms.faction.def.techLevel >= TechLevel.Industrial)
            {
                //Log.Message("CaveBiome: Will use cave-drop as strategy");
                parms.raidArrivalMode = DefDatabase<PawnsArrivalModeDef>.GetNamedSilentFail("CaveDrop");
            }
            else
            {
                //Log.Message("CaveBiome: Will use edge walkin as strategy");
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            }
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
                if (Rand.Bool)
                {
                    parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                }
                else
                {
                    parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamedSilentFail("ImmediateAttackSmart");
                }
            }
            else
            {
                parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamedSilentFail("StageThenAttack");
            }
        }
    }
}
