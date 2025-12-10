using RimWorld;
using Verse;

namespace CaveBiome;

/// <summary>
///     Building_AnimalCorpsesGenerator class.
/// </summary>
/// <author>Rikiki</author>
/// <permission>
///     Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
///     Remember learning is always better than just copy/paste...
/// </permission>
public class Building_AnimalCorpsesGenerator : Building
{
    protected override void Tick()
    {
        base.Tick();

        generateAnimalCorpses();
        Destroy();
    }

    private void generateAnimalCorpses()
    {
        var animalCorpsesNumber = Rand.Range(3, 7);
        for (var corpseIndex = 0; corpseIndex < animalCorpsesNumber; corpseIndex++)
        {
            var spawnCellIsFound = CellFinder.TryFindRandomCellNear(Position, Map, 5, validator, out var spawnCell);
            if (!spawnCellIsFound)
            {
                continue;
            }

            PawnKindDef animalKindDef;
            var animalKindSelector = Rand.Value;
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

            Building_VillagerCorpsesGenerator.SpawnPawnCorpse(Map, spawnCell, animalKindDef, null,
                Rand.Range(5f, 20f) * GenDate.TicksPerDay);
            continue;

            bool validator(IntVec3 cell)
            {
                if (!cell.Standable(Map))
                {
                    return false;
                }

                foreach (var thing in Map.thingGrid.ThingsListAt(cell))
                {
                    if (thing is Corpse)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}