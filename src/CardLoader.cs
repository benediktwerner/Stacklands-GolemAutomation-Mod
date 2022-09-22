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
                if (c.CardType == CardType.Structures)
                {
                    card.PickupSoundGroup = PickupSoundGroup.Heavy;
                }
                if (c.Init != null)
                    c.Init(card);
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
                card.NeedsExactMatch = idea.NeedsExactMatch;
                foreach (var sub in card.Subprints)
                {
                    var term = Consts.PREFIX + sub.StatusTerm.Replace(" ", "").ToLower();
                    AddTranslation(term, sub.StatusTerm);
                    sub.StatusTerm = term;
                }
                allCards.Add(card);
            }
        }

        public static readonly Card[] Cards = new[]
        {
            Card.Create<Resource>(Consts.PAPER, "Paper", "This is paper", 1, CardType.Resources),
            Card.Create<StoragePlace>(
                Consts.STORAGE_PLACE,
                "Storage Place",
                "Use a filter on it to specify resources that should automatically be placed here.\n\nOther buildings can be placed on top.",
                5,
                CardType.Structures,
                building: true
            ),
            Card.Create<Filter>(
                Consts.FILTER,
                "Filter",
                "Specifies allowed cards",
                5,
                CardType.Resources
            ),
            Card.Create<LocationGlyph>(
                Consts.LOCATION_GLYPH,
                "Location Glyph",
                "Place building on top to bind",
                7,
                CardType.Resources
            ),
            Card.Create<AreaGlyph>(
                Consts.AREA_GLYPH,
                "Area Glyph",
                "Makes the Golem collect ressources from nearby storage piles instead of a bound location.",
                10,
                CardType.Resources
            ),
            Card.Create<Resource>(
                Consts.CRASHED_SPACESHIP,
                "Crashed Spaceship",
                "This doesn't do anything anymore and only exists for backwards compatibility.",
                5,
                CardType.Resources
            ),
            Card.Create<HarvestableWithIdea>(
                Consts.BROKEN_GOLEM,
                "Broken Golem",
                "Human shaped pile of iron and gold. But why does it glow blue?",
                3,
                CardType.Locations,
                init: (c) =>
                {
                    c.Amount = 3;
                    c.HarvestTime = 15f;
                    c.MyCardBag = new CardBag
                    {
                        CardBagType = CardBagType.Chances,
                        Chances = new List<CardChance>
                        {
                            new CardChance { Id = Consts.IRON_BAR, Chance = 2 },
                            new CardChance { Id = Consts.GOLD_BAR, Chance = 2 },
                            new CardChance { Id = Consts.GLASS, Chance = 2 },
                            new CardChance { Id = Consts.ENERGY_CORE, Chance = 1 },
                            new CardChance { Id = Consts.LOCATION_GLYPH, Chance = 2 },
                            new CardChance { Id = Consts.GOLEM_MOD, Chance = 1 },
                        }
                    };
                    c.IdeaDrops = new[]
                    {
                        Consts.Idea(Consts.GOLEM),
                        Consts.Idea(Consts.LOCATION_GLYPH),
                        Consts.Idea(Consts.ENERGY_COMBOBULATOR),
                        Consts.Idea(Consts.ENERGY_CORE),
                        Consts.Idea(Consts.GOLEM_MOD),
                        Consts.Idea(Consts.GOLEM_MOD_SPEED),
                        Consts.Idea(Consts.GOLEM_MOD_SELL),
                        Consts.Idea(Consts.GOLEM_MOD_COUNTER),
                        Consts.Idea(Consts.GOLEM_MOD_CRAFTER),
                        Consts.Idea(Consts.GOLEM_L),
                        Consts.Idea(Consts.GOLEM_XL),
                    };

                    const string statusTerm = Consts.BROKEN_GOLEM + ".status";
                    c.StatusTerm = statusTerm;
                    AddTranslation(statusTerm, "Dismantling broken golem");
                }
            ),
            Card.Create<HarvestableWithIdea>(
                Consts.BROKEN_GOLEM_XL,
                "Humongous Broken Golem",
                "House-sized pile of iron, gold, and energy",
                5,
                CardType.Locations,
                init: (c) =>
                {
                    c.Amount = 10;
                    c.HarvestTime = 30f;
                    c.MyCardBag = new CardBag
                    {
                        CardBagType = CardBagType.Chances,
                        Chances = new List<CardChance>
                        {
                            new CardChance { Id = Consts.IRON_BAR, Chance = 5 },
                            new CardChance { Id = Consts.GOLD_BAR, Chance = 5 },
                            new CardChance { Id = Consts.GLASS, Chance = 5 },
                            new CardChance { Id = Consts.ENERGY_CORE, Chance = 3 },
                            new CardChance { Id = Consts.LOCATION_GLYPH, Chance = 3 },
                            new CardChance { Id = Consts.GOLEM_MOD, Chance = 1 },
                            new CardChance { Id = Consts.GOLEM_XL_LEFT_ARM, Chance = 1 },
                            new CardChance { Id = Consts.GOLEM_XL_RIGHT_ARM, Chance = 1 },
                            new CardChance { Id = Consts.GOLEM_XL_LEGS, Chance = 1 },
                        }
                    };
                    c.IdeaDrops = new[]
                    {
                        Consts.Idea(Consts.GOLEM_XL),
                        Consts.Idea(Consts.GOLEM_XL_LEFT_ARM),
                        Consts.Idea(Consts.GOLEM_XL_RIGHT_ARM),
                        Consts.Idea(Consts.GOLEM_XL_LEGS),
                    };

                    const string statusTerm = Consts.BROKEN_GOLEM_XL + ".status";
                    c.StatusTerm = statusTerm;
                    AddTranslation(statusTerm, "Dismantling broken golem");
                }
            ),
            Card.Create<Resource>(
                Consts.ENERGY_COMBOBULATOR,
                "Energy Combobulator",
                "Combobulates gold and glass into energy cores",
                30,
                CardType.Structures,
                building: true
            ),
            Card.Create<Resource>(
                Consts.ENERGY_CORE,
                "Energy Core",
                "Core building block of every golem",
                10,
                CardType.Resources
            ),
            Card.Create<Resource>(
                Consts.GOLEM_XL_LEFT_ARM,
                "Humongous Left Arm",
                "Building block for a humongous golem",
                30,
                CardType.Resources
            ),
            Card.Create<Resource>(
                Consts.GOLEM_XL_RIGHT_ARM,
                "Humongous Right Arm",
                "Building block for a humongous golem",
                30,
                CardType.Resources
            ),
            Card.Create<Resource>(
                Consts.GOLEM_XL_LEGS,
                "Humongous Legs",
                "Building block for a humongous golem",
                30,
                CardType.Resources
            ),
            Card.Create<Golem>(
                Consts.GOLEM,
                "Golem",
                "Moves one card from one stack to another. Use two Location Glyphs to specify start and end or a single one to dump cards next to the golem. Use a filter to restrict what it picks up. Has space for 2 golem modules.",
                20,
                CardType.Structures,
                building: true
            ),
            Card.Create<Golem>(
                Consts.GOLEM_L,
                "Large Golem",
                "Slower than a golem but can carry 5 cards at once and load 4 modules",
                50,
                CardType.Structures,
                building: true,
                init: (golem) =>
                {
                    golem.BaseSpeedModifier = 2f;
                    golem.SpeedModifier = 2f;
                    golem.CarryingCapacity = 5;
                    golem.ModulesLeft = 4;
                }
            ),
            Card.Create<Golem>(
                Consts.GOLEM_XL,
                "Humongous Golem",
                "Slower than a Large Golem but can carry 10 cards at once and load 5 modules",
                150,
                CardType.Structures,
                building: true,
                init: (golem) =>
                {
                    golem.BaseSpeedModifier = 3f;
                    golem.SpeedModifier = 3f;
                    golem.CarryingCapacity = 10;
                    golem.ModulesLeft = 5;
                }
            ),
            Card.Create<Golem>(
                Consts.GOLEM_MOD,
                "Golem Module",
                "Used to craft specific golem modules",
                10,
                CardType.Resources
            ),
            Card.Create<GolemModuleSell>(
                Consts.GOLEM_MOD_SELL,
                "Selling Module",
                "Allows the golem to sell stuff. When transfering to a full chest, excess gold will be voided to avoid overflows.",
                15,
                CardType.Resources
            ),
            Card.Create<GolemModuleSpeed>(
                Consts.GOLEM_MOD_SPEED,
                "Speed Module",
                "Makes the golem work twice as fast",
                15,
                CardType.Resources
            ),
            Card.Create<GolemModuleCounter>(
                Consts.GOLEM_MOD_COUNTER,
                "Counter Module",
                "Allows the golem to count (and only pick up cards from the source stack that are over the limit).\n\nPlace coins on it to set the count.",
                10,
                CardType.Resources
            ),
            Card.Create<GolemModuleCrafter>(
                Consts.GOLEM_MOD_CRAFTER,
                "Crafter Module",
                "Allows the golem to craft a recepie.\n\nPlace recepie on top to configure.",
                20,
                CardType.Resources
            ),
        };

        public static readonly Idea[] Ideas = new[]
        {
            new Idea(
                Consts.PAPER,
                BlueprintGroup.Resources,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.BRICK, Consts.SUGAR_CANE },
                        ResultCard = Consts.PAPER,
                        CardsToRemove = new[] { Consts.SUGAR_CANE },
                        Time = 10.0f,
                        StatusTerm = "Making Paper",
                    },
                }
            ),
            new Idea(
                Consts.STORAGE_PLACE,
                BlueprintGroup.Building,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.PLANK,
                            Consts.STICK,
                            Consts.STICK,
                            Consts.ANY_VILL
                        },
                        ResultCard = Consts.STORAGE_PLACE,
                        Time = 10.0f,
                        StatusTerm = "Making Storage Place",
                    }
                }
            ),
            new Idea(
                Consts.FILTER,
                BlueprintGroup.Resources,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.IRON_BAR, Consts.FLINT },
                        ResultCard = Consts.FILTER,
                        Time = 5.0f,
                        StatusTerm = "Making Filter",
                    },
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.FILTER, Consts.ANY_VILL },
                        ResultCard = Consts.FILTER,
                        Time = 5.0f,
                        StatusTerm = "Cleaning Filter",
                    },
                }
            ),
            new Idea(
                Consts.LOCATION_GLYPH,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.IRON_BAR, Consts.STICK, Consts.ANY_VILL },
                        ResultCard = Consts.LOCATION_GLYPH,
                        Time = 10.0f,
                        StatusTerm = "Making Location Glyph",
                    },
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.LOCATION_GLYPH, Consts.ANY_VILL },
                        ResultCard = Consts.LOCATION_GLYPH,
                        Time = 5.0f,
                        StatusTerm = "Unbinding Glyph",
                    },
                }
            ),
            new Idea(
                Consts.AREA_GLYPH,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.LOCATION_GLYPH, Consts.ROPE },
                        ResultCard = Consts.AREA_GLYPH,
                        Time = 10.0f,
                        StatusTerm = "Making Area Glyph",
                    },
                }
            ),
            new Idea(
                Consts.ENERGY_COMBOBULATOR,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.BRICK,
                            Consts.BRICK,
                            Consts.BRICK,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR,
                            Consts.GLASS,
                            Consts.GLASS,
                            Consts.GLASS,
                            Consts.PAPER,
                            Consts.ANY_VILL,
                            Consts.ANY_VILL
                        },
                        ResultCard = Consts.ENERGY_COMBOBULATOR,
                        Time = 30.0f,
                        StatusTerm = "Making Energy Combobulator",
                    },
                }
            ),
            new Idea(
                Consts.ENERGY_CORE,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_COMBOBULATOR,
                            Consts.GOLD_BAR,
                            Consts.GLASS,
                        },
                        ResultCard = Consts.ENERGY_CORE,
                        Time = 10.0f,
                        StatusTerm = "Making Golem Core",
                    },
                },
                needsExactMatch: false
            ),
            new Idea(
                Consts.GOLEM,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.IRON_BAR,
                            Consts.GLASS,
                            Consts.ANY_VILL
                        },
                        ResultCard = Consts.GOLEM,
                        Time = 10.0f,
                        StatusTerm = "Making Golem",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_L,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.ENERGY_CORE,
                            Consts.ENERGY_CORE,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.GOLD_BAR,
                            Consts.GLASS,
                            Consts.ANY_VILL
                        },
                        ResultCard = Consts.GOLEM_L,
                        Time = 20.0f,
                        StatusTerm = "Making Large Golem",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_XL,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.ENERGY_CORE,
                            Consts.ENERGY_CORE,
                            Consts.GOLEM_XL_LEFT_ARM,
                            Consts.GOLEM_XL_RIGHT_ARM,
                            Consts.GOLEM_XL_LEGS,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR,
                            Consts.GLASS,
                            Consts.GLASS,
                            Consts.ANY_VILL
                        },
                        ResultCard = Consts.GOLEM_XL,
                        Time = 30.0f,
                        StatusTerm = "Making Humongous Golem",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_XL_LEFT_ARM,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                        },
                        ResultCard = Consts.GOLEM_XL_LEFT_ARM,
                        Time = 30.0f,
                        StatusTerm = "Making Humongous Left Arm",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_XL_RIGHT_ARM,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR,
                        },
                        ResultCard = Consts.GOLEM_XL_RIGHT_ARM,
                        Time = 30.0f,
                        StatusTerm = "Making Humongous Right Arm",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_XL_LEGS,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.ENERGY_CORE,
                            Consts.BRICK,
                            Consts.BRICK,
                            Consts.BRICK,
                            Consts.BRICK,
                        },
                        ResultCard = Consts.GOLEM_XL_LEGS,
                        Time = 30.0f,
                        StatusTerm = "Making Humongous Legs",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_MOD,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.ENERGY_CORE, Consts.PAPER },
                        ResultCard = Consts.GOLEM_MOD,
                        Time = 10.0f,
                        StatusTerm = "Making Golem Module",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_MOD_SELL,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.GOLEM_MOD,
                            Consts.COIN,
                            Consts.COIN,
                            Consts.COIN,
                            Consts.COIN,
                            Consts.COIN,
                            Consts.PLANK,
                            Consts.PLANK
                        },
                        ResultCard = Consts.GOLEM_MOD_SELL,
                        Time = 10.0f,
                        StatusTerm = "Making Selling Module",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_MOD_SPEED,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.GOLEM_MOD,
                            Consts.SUGAR,
                            Consts.SUGAR,
                            Consts.SUGAR,
                            Consts.GOLD_BAR,
                            Consts.GOLD_BAR
                        },
                        ResultCard = Consts.GOLEM_MOD_SPEED,
                        Time = 10.0f,
                        StatusTerm = "Making Speed Module",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_MOD_COUNTER,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[] { Consts.GOLEM_MOD, Consts.PAPER, Consts.COIN },
                        ResultCard = Consts.GOLEM_MOD_COUNTER,
                        Time = 10.0f,
                        StatusTerm = "Making Counter Module",
                    },
                }
            ),
            new Idea(
                Consts.GOLEM_MOD_CRAFTER,
                Consts.BLUEPRINT_GROUP_GOLEM,
                new List<Subprint>
                {
                    new Subprint
                    {
                        RequiredCards = new[]
                        {
                            Consts.GOLEM_MOD,
                            Consts.BRICK,
                            Consts.BRICK,
                            Consts.IRON_BAR,
                            Consts.IRON_BAR,
                            Consts.ROPE
                        },
                        ResultCard = Consts.GOLEM_MOD_CRAFTER,
                        Time = 10.0f,
                        StatusTerm = "Making Crafter Module",
                    },
                }
            ),
        };
    }
}
