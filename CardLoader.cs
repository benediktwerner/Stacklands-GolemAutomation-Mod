using System.Collections.Generic;
using UnityEngine;

namespace GolemAutomation
{
    static class CardLoader
    {
        private static GameObject cardContainer;

        public static void AddTranslation(string id, string text)
        {
            var term = new SokTerm(SokLoc.FallbackSet, id, text);
            if (SokLoc.instance != null)
                SokLoc.instance.CurrentLocSet.TermLookup[id] = term;
            SokLoc.FallbackSet.TermLookup[id] = term;
        }

        public static void AddCards(List<CardData> allCards)
        {
            cardContainer = new GameObject("CardContainer - " + PluginInfo.PLUGIN_NAME);
            cardContainer.gameObject.SetActive(false);

            foreach (var c in Cards)
            {
                var go = new GameObject(c.Id);
                go.transform.SetParent(cardContainer.transform);
                var card = (CardData)go.AddComponent(c.ScriptType);
                card.Id = c.Id;
                card.NameTerm = c.Id;
                AddTranslation(card.NameTerm, c.Name);
                card.DescriptionTerm = c.Id + "_desc";
                AddTranslation(card.DescriptionTerm, c.Description);
                card.Value = c.Value;
                card.MyCardType = c.CardType;
                card.IsBuilding = c.IsBuilding;
                allCards.Add(card);
            }

            foreach (var idea in Ideas)
            {
                var id = Consts.Idea(idea.Name);
                var go = new GameObject(id);
                go.transform.SetParent(cardContainer.transform);
                var card = go.AddComponent<Blueprint>();
                card.Id = id;
                card.NameTerm = idea.Name;
                card.MyCardType = CardType.Ideas;
                card.BlueprintGroup = idea.Group;
                card.Subprints = idea.Subprints;
                allCards.Add(card);
            }
        }

        public static readonly Card[] Cards = new[]
        {
            new Card(Consts.PAPER, "Paper", "This is paper", 5, CardType.Resources),
            new Card(Consts.STORAGE_PLACE, "Storage Place", "Use a filter on it to specify which resources should automatically be placed here\n\nOther buildings can be placed on top", 1, CardType.Structures, typeof(StoragePlace), building: true),
            new Card(Consts.FILTER, "Filter", "List to specify allowed cards", 5, CardType.Resources, typeof(Filter)),
            new Card(Consts.LOCATION_GLYPH, "Location Glyph", "Place building on top to bind", 7, CardType.Resources, typeof(LocationGlyph)),
            new Card(Consts.GOLEM, "Golem", "Moves cards from one Storage Space to another. Use two Location Glyphs to program.", 7, CardType.Structures, typeof(Golem), building: true),
        };

        public static readonly Idea[] Ideas = new[]
        {
            new Idea(Consts.PAPER, BlueprintGroup.Resources, new List<Subprint>
            {
                new Subprint {
                    RequiredCards = new[] { Consts.SUGAR, Consts.SUGAR, Consts.ANY_VILL },
                    ResultCard = Consts.PAPER,
                    Time = 10.0f,
                    StatusTerm = "Making Paper",
                }
            }),
            new Idea(Consts.STORAGE_PLACE, BlueprintGroup.Building, new List<Subprint>
            {
                new Subprint {
                    RequiredCards = new[] { Consts.ROPE, Consts.STICK, Consts.STICK, Consts.ANY_VILL },
                    ResultCard = Consts.STORAGE_PLACE,
                    Time = 10.0f,
                    StatusTerm = "Making Storage Place",
                }
            }),
            new Idea(Consts.FILTER, BlueprintGroup.Resources, new List<Subprint>
            {
                new Subprint {
                    RequiredCards = new[] { Consts.PAPER, Consts.CHARCOAL },
                    ResultCard = Consts.FILTER,
                    Time = 5.0f,
                    StatusTerm = "Making Filter",
                },
                new Subprint {
                    RequiredCards = new[] { Consts.FILTER, Consts.ANY_VILL },
                    ResultCard = Consts.FILTER,
                    Time = 5.0f,
                    StatusTerm = "Cleaning Filter",
                },
            }),
            new Idea(Consts.LOCATION_GLYPH, BlueprintGroup.Resources, new List<Subprint>
            {
                new Subprint {
                    RequiredCards = new[] { Consts.IRON_BAR, Consts.STICK, Consts.ANY_VILL },
                    ResultCard = Consts.LOCATION_GLYPH,
                    Time = 10.0f,
                    StatusTerm = "Making Location Glyph",
                },
                new Subprint {
                    RequiredCards = new[] { Consts.LOCATION_GLYPH, Consts.ANY_VILL },
                    ResultCard = Consts.LOCATION_GLYPH,
                    Time = 5.0f,
                    StatusTerm = "Unbinding Glyph",
                },
            }),
        };
    }
}
