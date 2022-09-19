using System;
using System.Collections.Generic;
using UnityEngine;

namespace GolemAutomation
{
    class Card
    {
        public readonly string Id;
        public readonly string Name;
        public readonly string Description;
        public readonly int Value;
        public readonly CardType CardType;
        public readonly Type ScriptType;
        public readonly bool IsBuilding;
        public readonly Action<CardData> Init;

        public Card(
            string id,
            string name,
            string description,
            int value,
            CardType cardType,
            Type scriptType = null,
            bool building = false,
            Action<CardData> init = null
        )
        {
            Id = id;
            Name = name;
            Description = description;
            Value = value;
            CardType = cardType;
            ScriptType = scriptType ?? typeof(CardData);
            IsBuilding = building;
            Init = init;
        }

        public static Card Create<T>(
            string id,
            string name,
            string description,
            int value,
            CardType cardType,
            bool building = false,
            Action<T> init = null
        ) where T : CardData
        {
            return new Card(
                id,
                name,
                description,
                value,
                cardType,
                typeof(T),
                building,
                init == null ? null : (c) => init((T)c)
            );
        }

        public static bool IsAnimal(CardData card)
        {
            return card.MyCardType == CardType.Fish || card.MyCardType == CardType.Mobs;
        }

        public static bool IsAlive(GameCard card) => IsAnimal(card.CardData);

        public static bool IsCurrencyStack(CardData card)
        {
            while (card != null)
            {
                if (card.Id != Card.Currency)
                    return false;
                card = card.MyGameCard.Child?.CardData;
            }
            return true;
        }

        public static void Restack(List<GameCard> cards)
        {
            if (cards.Count == 0)
                return;
            for (var i = 1; i + 1 < cards.Count; i++)
            {
                cards[i].Parent = cards[i - 1];
                cards[i].Child = cards[i + 1];
            }
            cards[0].Parent = null;
            cards[cards.Count - 1].Child = null;
            if (cards.Count > 1)
            {
                cards[0].Child = cards[1];
                cards[cards.Count - 1].Parent = cards[cards.Count - 2];
            }
        }

        public static void BounceTo(GameCard card, GameCard to)
        {
            // card.BounceTarget = to;
            to.Child = card;
            card.Parent = to;
            var vec = to.transform.position - card.transform.position;
            card.Velocity = new Vector3(vec.x * 4f, 7f, vec.z * 4f);
        }

        public static void Parent(GameCard parent, GameCard child)
        {
            parent.Child = child;
            child.Parent = parent;
        }

        public static void InsertBelow(GameCard parent, GameCard child)
        {
            var leaf = child.GetLeafCard();
            if (parent.Child != null)
                parent.Child.Parent = leaf;
            leaf.Child = parent.Child;
            parent.Child = child;
            child.Parent = parent;
        }

        public static GameCard Sell(List<GameCard> cards, Vector3 pos)
        {
            if (cards.Count == 0)
                return null;
            var value = 0;
            var first = cards[0];
            do
            {
                value += first.CardData.Value;
                WorldManager.instance.CreateSmoke(first.transform.position);
                var nxt = first.Child;
                UnityEngine.Object.Destroy(first.gameObject);
                first = nxt;
            } while (first != null);
            var goldStack = WorldManager.instance.CreateCardStack(pos, value, Currency, false);
            if (goldStack != null)
            {
                AudioManager.me.PlaySound2D(
                    AudioManager.me.Coin,
                    UnityEngine.Random.Range(0.8f, 1.2f),
                    0.8f
                );
                return goldStack.GetRootCard();
            }
            return null;
        }

        public static GameCard PopAndGetChild(GameCard card)
        {
            var child = card.Child;
            if (child != null)
                child.Parent = card.Parent;
            card.Parent.Child = child;
            card.Parent = null;
            card.Child = null;
            card.SendIt();
            return child;
        }

        public static string Currency =>
            WorldManager.instance.CurrentBoard.Id == Consts.MAINLAND ? Consts.COIN : Consts.SHELL;
    }
}
