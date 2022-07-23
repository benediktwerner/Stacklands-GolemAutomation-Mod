using System;

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

        public Card(string id, string name, string description, int value, CardType cardType, Type scriptType = null, bool building = false)
        {
            Id = id;
            Name = name;
            Description = description;
            Value = value;
            CardType = cardType;
            ScriptType = scriptType ?? typeof(CardData);
            IsBuilding = building;
        }

        public static bool IsAlive(CardData card)
        {
            return card.MyCardType == CardType.Fish || card.MyCardType == CardType.Mobs;
        }

        public static bool IsAlive(GameCard card) => IsAlive(card.CardData);
    }
}
