using RimWorld;
using Verse;

namespace CaveBiome;

/// <summary>
///     Building_VillagerCorpsesGenerator class.
/// </summary>
/// <author>Rikiki</author>
/// <permission>
///     Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
///     Remember learning is always better than just copy/paste...
/// </permission>
public class Building_VillagerCorpsesGenerator : Building
{
    protected override void Tick()
    {
        base.Tick();

        GenerateVillagerCorpses();
        Destroy();
    }

    private void GenerateVillagerCorpses()
    {
        var faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("TribeCivil"));
        SpawnPawnCorpse(Map, Position, PawnKindDef.Named("Tribal_ChiefMelee"), faction, GenDate.TicksPerDay, true);
        SpawnPawnCorpse(Map, Position + new IntVec3(2, 0, 2), PawnKindDef.Named("Tribal_Warrior"), faction,
            GenDate.TicksPerDay, true);
        SpawnPawnCorpse(Map, Position + new IntVec3(2, 0, -2), PawnKindDef.Named("Tribal_Warrior"), faction,
            GenDate.TicksPerDay, true);
        SpawnPawnCorpse(Map, Position + new IntVec3(-2, 0, -2), PawnKindDef.Named("Tribal_Warrior"), faction,
            GenDate.TicksPerDay, true);
        SpawnPawnCorpse(Map, Position + new IntVec3(-2, 0, 2), PawnKindDef.Named("Tribal_Warrior"), faction,
            GenDate.TicksPerDay, true);
    }

    public static void SpawnPawnCorpse(Map map, IntVec3 spawnCell, PawnKindDef pawnKindDef, Faction faction,
        float rotProgressInTicks, bool removeEquipment = false)
    {
        var pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
        GenSpawn.Spawn(pawn, spawnCell, map);
        if (removeEquipment)
        {
            pawn.equipment.DestroyAllEquipment();
            pawn.inventory.DestroyAll();
        }

        KillAndRotPawn(pawn, rotProgressInTicks);
    }

    private static void KillAndRotPawn(Pawn pawn, float rotProgressInTicks)
    {
        HealthUtility.DamageUntilDead(pawn);
        foreach (var thing in pawn.Position.GetThingList(pawn.MapHeld))
        {
            if (!thing.def.defName.Contains("Corpse"))
            {
                continue;
            }

            var rotComp = thing.TryGetComp<CompRottable>();
            if (rotComp != null)
            {
                rotComp.RotProgress = rotProgressInTicks;
            }
        }
    }
}