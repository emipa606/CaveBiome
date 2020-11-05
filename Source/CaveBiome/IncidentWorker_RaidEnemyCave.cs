
using Verse;
using RimWorld;

namespace CaveBiome
{
    public class IncidentWorker_RaidEnemyCave : IncidentWorker_RaidEnemy
    {
        public override void ResolveRaidArriveMode(IncidentParms parms)
        {
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
