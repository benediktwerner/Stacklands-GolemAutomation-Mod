using HarmonyLib;

namespace GolemAutomation
{
    public class Plugin : Mod
    {
        public static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Harmony.PatchAll(typeof(Patches));
        }

        public override void Ready()
        {
            var jungleChances = ((Harvestable)WorldManager.instance.GameDataLoader.idToCard[Consts.JUNGLE])
                .MyCardBag
                .Chances;
            jungleChances.Add(new CardChance { Id = Consts.BROKEN_GOLEM, Chance = 1 });
            jungleChances.Add(new CardChance { Id = Consts.BROKEN_GOLEM_XL, Chance = 1 });
            AddBoosterIdea(
                SetCardBagType.AdvancedBuildingIdea,
                Consts.Idea(Consts.FILTER),
                Consts.Idea(Consts.STORAGE_PLACE)
            );
            AddBoosterIdea(
                SetCardBagType.Island_AdvancedIdea,
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
                SetCardBagType.Island_AdvancedBuildingIdea,
                Consts.Idea(Consts.ENERGY_COMBOBULATOR),
                Consts.Idea(Consts.GOLEM),
                Consts.Idea(Consts.GOLEM_L)
            );
        }

        public static void AddBoosterIdea(SetCardBagType bag, params string[] cardIds)
        {
            var loader = WorldManager.instance.GameDataLoader;
            foreach (var cardId in cardIds)
            {
                loader.AddCardToSetCardBag(bag, cardId, 1);
            }
        }
    }
}
