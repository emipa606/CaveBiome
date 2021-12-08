using Verse;

namespace CaveBiome;

public class IncidentWorker_DefoliatorShipPartCrashInCave : IncidentWorker_ShipPartCrashInCave
{
    protected override int CountToSpawn => Rand.RangeInclusive(1, 1);
}