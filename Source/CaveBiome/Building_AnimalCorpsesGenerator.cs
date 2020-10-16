using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.Sound; // Needed when you do something with the Sound

namespace CaveBiome
{
    /// <summary>
    /// Building_AnimalCorpsesGenerator class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class Building_AnimalCorpsesGenerator : Building
    {
        public override void Tick()
        {
            base.Tick();
            
            GenerateAnimalCorpses();
            Destroy();
        }

        public void GenerateAnimalCorpses()
        {
            int animalCorpsesNumber = Rand.Range(3, 7);
            for (int corpseIndex = 0; corpseIndex < animalCorpsesNumber; corpseIndex++)
            {
                bool validator(IntVec3 cell)
                {
                    if (cell.Standable(Map) == false)
                    {
                        return false;
                    }
                    foreach (Thing thing in Map.thingGrid.ThingsListAt(cell))
                    {
                        if (thing is Corpse)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                IntVec3 spawnCell = IntVec3.Invalid;
                bool spawnCellIsFound = CellFinder.TryFindRandomCellNear(Position, Map, 5, validator, out spawnCell);
                if (spawnCellIsFound)
                {
                    PawnKindDef animalKindDef;
                    float animalKindSelector = Rand.Value;
                    if (animalKindSelector < 0.33f)
                    {
                        animalKindDef = PawnKindDef.Named("Muffalo");
                    }
                    else if (animalKindSelector < 0.66f)
                    {
                        animalKindDef = PawnKindDef.Named("Caribou");
                    }
                    else
                    {
                        animalKindDef = PawnKindDef.Named("Deer");
                    }
                    Building_VillagerCorpsesGenerator.SpawnPawnCorpse(Map, spawnCell, animalKindDef, null, Rand.Range(5f, 20f) * GenDate.TicksPerDay);
                }
            }
        }
    }
}
