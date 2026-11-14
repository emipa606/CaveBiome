using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CaveBiome;

public class BiomeWorker_Cave : BiomeWorker
{
    public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
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

        if (tile.elevation is < 1000f or > 3000f)
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