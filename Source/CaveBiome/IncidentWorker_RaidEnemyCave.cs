
using Verse;
using RimWorld;

namespace CaveBiome
{
    public class IncidentWorker_RaidEnemyCave : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.Biome == Util_CaveBiome.CaveBiomeDef)
            {
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                if ((parms.raidStrategy != null)
                    && (parms.raidStrategy.defName == "Siege"))
                {
                    parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                }
                return base.TryExecute(parms);
            }
            else
            {
                return base.TryExecute(parms);
            }
        }
    }
}
