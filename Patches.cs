using HarmonyLib;
using System.Collections.Generic;
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
                if (card.MyBoard.IsCurrent && card.CardData is StoragePlace f && card.Parent == null)
                {
                    Vector3 vec = card.transform.position - myCard.transform.position;
                    vec.y = 0f;
                    var dist = vec.sqrMagnitude;
                    if (dist <= 9f && dist < minDist && !card.BeingDragged && f.filter.Contains(myCard.CardData.Id) && CanAutoStack(card, myCard))
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
                if (child.Child == newCard) return false;
                child = child.Child;
            }
            return child == filter || child.CardData.CanHaveCardOnTop(newCard.CardData);
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Restack))]
        [HarmonyPrefix]
        public static void RestackPrefix(List<GameCard> cards, out GameCard __state)
        {
            __state = null;
            if (cards.Count == 0) return;
            var parent = cards[0].Parent;
            if (parent?.CardData.Id == Consts.STORAGE_PLACE) __state = parent;
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Restack))]
        [HarmonyPostfix]
        public static void RestackPostfix(List<GameCard> cards, GameCard __state)
        {
            if (__state != null)
            {
                cards[0].Parent = __state;
                __state.Child = cards[0];
            }
        }
    }
}
