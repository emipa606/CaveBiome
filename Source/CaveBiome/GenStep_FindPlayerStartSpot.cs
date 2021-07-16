using RimWorld;
using Verse;

namespace CaveBiome
{
    public class GenStep_CaveFindPlayerStartSpot : GenStep_FindPlayerStartSpot
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                // Use standard base function.
                base.Generate(map, parms);
                return;
            }

            if (!MapGenerator.PlayerStartSpot.IsValid)
            {
                base.Generate(map, parms);
            }
        }
    }
}