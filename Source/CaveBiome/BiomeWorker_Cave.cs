using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome;
// TODO: add solar panel placed near glowing crystal?
// TODO: add some insect hives with a limit of size? Modify hive class to limit reproduction?
// TODO: Try to correct incoming caravans establishing a new colony without visibility.
// TODO: correct cave well light inside roofed buildings.
// TODO: use patches for mortar/incidents/solarpanels.
// TODO: remove shipPartInCave

public class BiomeWorker_Cave : BiomeWorker
{
    public override float GetScore(Tile tile, int tileID)
    {
        if (tile.hilliness != Hilliness.Mountainous
            && tile.hilliness != Hilliness.LargeHills)
        {
            return -100f;
        }

        if (tile.elevation <= 0f)
        {
            return -100f;
        }

        if (tile.elevation < 1000f
            || tile.elevation > 3000f)
        {
            return 0f;
        }

        if (Rand.Value < 0.15f)
        {
            return 100f;
        }

        return -100f;
    }
}