using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace CaveBiome
{
    public class IncidentWorker_ResourcePodCrashInCave : IncidentWorker_ResourcePodCrash
    {
        private const int MaxStacks = 8;
        private const float MaxMarketValue = 40f;

        private static ThingDef RandomPodContentsDef()
        {
            bool isLeather(ThingDef d)
            {
                return d.category == ThingCategory.Item && d.thingCategories != null &&
                       d.thingCategories.Contains(ThingCategoryDefOf.Leathers);
            }

            bool isMeat(ThingDef d)
            {
                return d.category == ThingCategory.Item && d.thingCategories != null &&
                       d.thingCategories.Contains(ThingCategoryDefOf.MeatRaw);
            }

            var numLeathers = DefDatabase<ThingDef>.AllDefs.Where(isLeather).Count();
            var numMeats = DefDatabase<ThingDef>.AllDefs.Where(isMeat).Count();
            return (
                from d in DefDatabase<ThingDef>.AllDefs
                where d.category == ThingCategory.Item && d.tradeability == Tradeability.Sellable &&
                      d.equipmentType == EquipmentType.None && d.BaseMarketValue >= 1f &&
                      d.BaseMarketValue < MaxMarketValue && !d.HasComp(typeof(CompHatcher))
                select d).RandomElementByWeight(delegate(ThingDef d)
            {
                var num = 100f;
                if (isLeather(d))
                {
                    num *= 5f / numLeathers;
                }

                if (isMeat(d))
                {
                    num *= 5f / numMeats;
                }

                return num;
            });
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            if (map.Biome != Util_CaveBiome.CaveBiomeDef)
            {
                return TryExecute(parms);
            }

            var thingDef = RandomPodContentsDef();
            var list = new List<Thing>();
            var num = (float) Rand.Range(150, 900);
            do
            {
                var thing = ThingMaker.MakeThing(thingDef);
                var num2 = Rand.Range(20, 40);
                if (num2 > thing.def.stackLimit)
                {
                    num2 = thing.def.stackLimit;
                }

                if (num2 * thing.def.BaseMarketValue > num)
                {
                    num2 = Mathf.FloorToInt(num / thing.def.BaseMarketValue);
                }

                if (num2 == 0)
                {
                    num2 = 1;
                }

                thing.stackCount = num2;
                list.Add(thing);
                num -= num2 * thingDef.BaseMarketValue;
            } while (list.Count < MaxStacks && num > thingDef.BaseMarketValue);

            TryFindDropPodSpot(map, out var intVec);
            if (!intVec.IsValid)
            {
                return false;
            }

            DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true);
            Find.LetterStack.ReceiveLetter("LetterLabelCargoPodCrash".Translate(), "CargoPodCrash".Translate(),
                LetterDefOf.PositiveEvent, new GlobalTargetInfo(intVec, map));
            return true;
        }

        private void TryFindDropPodSpot(Map map, out IntVec3 spawnCell)
        {
            spawnCell = IntVec3.Invalid;
            var caveWellsList = map.listerThings.ThingsOfDef(Util_CaveBiome.CaveWellDef);
            foreach (var caveWell in caveWellsList.InRandomOrder())
            {
                if (!IsValidPositionToSpawnDropPod(map, caveWell.Position))
                {
                    continue;
                }

                spawnCell = caveWell.Position;
                return;
            }
        }

        private static bool IsValidPositionToSpawnDropPod(Map map, IntVec3 position)
        {
            if (position.InBounds(map) == false
                || position.Fogged(map)
                || position.Standable(map) == false
                || position.Roofed(map)
                && position.GetRoof(map).isThickRoof)
            {
                return false;
            }

            return true;
        }
    }
}