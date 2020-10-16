
using Verse;

namespace CaveBiome
{
    public class IncidentWorker_DefoliatorShipPartCrashInCave : IncidentWorker_ShipPartCrashInCave
    {
        protected override int CountToSpawn
        {
            get
            {
                return Rand.RangeInclusive(1, 1);
            }
        }
    }
}
