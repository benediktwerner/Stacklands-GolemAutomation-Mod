using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace GolemAutomation
{
    class Patches
    {
        [HarmonyPatch(typeof(GameDataLoader), nameof(GameDataLoader.LoadModCards))]
        [HarmonyPostfix]
        private static void AddCards(List<CardData> __result)
        {
            CardLoader.AddCards(__result);
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.StackSend))]
        [HarmonyPrefix]
        public static bool StackSend(WorldManager __instance, GameCard myCard)
        {
            GameCard target = null;
            float minDist = float.MaxValue;
            Vector3 velocity = Vector3.zero;

            foreach (var card in __instance.AllCards)
            {
                if (
                    card.MyBoard.IsCurrent
                    && card.CardData is StoragePlace f
                    && (card.Parent == null || card.Parent.CardData is HeavyFoundation)
                )
                {
                    Vector3 vec = card.transform.position - myCard.transform.position;
                    vec.y = 0f;
                    var dist = vec.sqrMagnitude;
                    if (
                        dist <= 9f
                        && dist < minDist
                        && !card.BeingDragged
                        && f.filter.Contains(myCard.CardData.Id)
                        && CanAutoStack(card, myCard)
                    )
                    {
                        target = card;
                        minDist = dist;
                        velocity = new Vector3(vec.x * 4f, 7f, vec.z * 4f);
                    }
                }
            }
            if (target != null)
            {
                myCard.BounceTarget = target;
                myCard.Velocity = velocity;
                return false;
            }

            return true;
        }

        public static bool CanAutoStack(GameCard filter, GameCard newCard)
        {
            var child = filter;
            while (child.Child != null)
            {
                if (child.Child == newCard)
                    return false;
                child = child.Child;
            }
            return child == filter || child.CardData.CanHaveCardOnTop(newCard.CardData);
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Restack))]
        [HarmonyPrefix]
        public static void RestackPrefix(List<GameCard> cards, out GameCard __state)
        {
            __state = null;
            if (cards.Count == 0)
                return;
            var parent = cards[0].Parent;
            if (parent?.CardData.Id == Consts.STORAGE_PLACE)
                __state = parent;
        }

        [HarmonyPatch(typeof(FishTrap), nameof(FishTrap.CompleteFishing))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendStackAfterFishing(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldloc_3),
                    new CodeMatch(OpCodes.Isinst, typeof(Animal)),
                    new CodeMatch(OpCodes.Brtrue)
                )
                .ThrowIfInvalid("Didn't find isAnimal check")
                .RemoveInstructions(3)
                .InstructionEnumeration();
        }

        [HarmonyPatch(typeof(CardData), nameof(CardData.Name), MethodType.Getter)]
        [HarmonyPostfix]
        public static void GolemName(ref string __result, CardData __instance)
        {
            if (__instance is Golem g)
            {
                __result += g.ModulePostfix;
            }
        }

        [HarmonyPatch(typeof(GameScreen), nameof(GameScreen.GetBlueprintGroupText))]
        [HarmonyPrefix]
        public static void GolemBlueprintGroupText(BlueprintGroup group, ref string __result, out bool __runOriginal)
        {
            if (group == Consts.BLUEPRINT_GROUP_GOLEM)
            {
                __result = "Golems";
                __runOriginal = false;
            }
            else
                __runOriginal = true;
        }

        [HarmonyPatch(typeof(GameScreen), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void GolemBlueprintGroup(GameScreen __instance)
        {
            __instance.groups.Add(Consts.BLUEPRINT_GROUP_GOLEM);
        }

        [HarmonyPatch(typeof(GameDataLoader), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void InsertDrops(GameDataLoader __instance)
        {
            var jungleChances = ((Harvestable)__instance.idToCard[Consts.JUNGLE]).MyCardBag.Chances;
            jungleChances.Add(new CardChance { Id = Consts.BROKEN_GOLEM, Chance = 1 });
            jungleChances.Add(new CardChance { Id = Consts.BROKEN_GOLEM_XL, Chance = 1 });
            AddBoosterIdea(
                __instance,
                SetCardBag.AdvancedBuildingIdea,
                Consts.Idea(Consts.FILTER),
                Consts.Idea(Consts.STORAGE_PLACE)
            );
            AddBoosterIdea(
                __instance,
                SetCardBag.Island_AdvancedIdea,
                Consts.Idea(Consts.PAPER),
                Consts.Idea(Consts.LOCATION_GLYPH),
                Consts.Idea(Consts.AREA_GLYPH),
                Consts.Idea(Consts.ENERGY_CORE),
                Consts.Idea(Consts.GOLEM_MOD),
                Consts.Idea(Consts.GOLEM_MOD_SELL),
                Consts.Idea(Consts.GOLEM_MOD_SPEED),
                Consts.Idea(Consts.GOLEM_MOD_COUNTER),
                Consts.Idea(Consts.GOLEM_MOD_CRAFTER)
            );
            AddBoosterIdea(
                __instance,
                SetCardBag.Island_AdvancedBuildingIdea,
                Consts.Idea(Consts.ENERGY_COMBOBULATOR),
                Consts.Idea(Consts.GOLEM),
                Consts.Idea(Consts.GOLEM_L)
            );
        }

        public static void AddBoosterIdea(GameDataLoader __instance, SetCardBag bag, params string[] cardIds)
        {
            var existing = __instance.GetStringForBag(bag);
            __instance.result[bag] = existing + ", " + cardIds.Join(delimiter: ", ");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SokLoc), nameof(SokLoc.SetLanguage))]
        public static void LanguageChanged(SokLoc __instance)
        {
            if (SokLoc.instance == null)
                return;

            foreach (var term in CardLoader.Translations)
            {
                SokLoc.instance.CurrentLocSet.TermLookup[term.Id] = term;
            }
        }
    }
}
