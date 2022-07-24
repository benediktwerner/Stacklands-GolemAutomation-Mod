using System;
using System.Linq;

namespace GolemAutomation
{
    class GolemModuleCrafter : GolemModule
    {
        public override bool CanInsert(Golem g) => g.CraftingRecipe.Length == 0 && Recipe.Length > 0 && !g.HasSellingModule;

        public override void Insert(Golem g)
        {
            g.CraftingRecipe = Recipe;
        }

        public override bool DetermineCanHaveCardsWhenIsRoot => true;
        public override bool CanHaveCardsWhileHasStatus() => true;

        public override bool CanHaveCard(CardData otherCard) => !Card.IsAnimal(otherCard) && !otherCard.IsBuilding;

        [ExtraData(Consts.GOLEM_MOD_CRAFTER + ".recepie")]
        public string Recipe = "";

        public void Start()
        {
            UpdateDescription();
        }

        public override void UpdateCard()
        {
            var recipe = ComputeCurrentRecipe();
            if (MyGameCard.Parent == null && recipe != null && recipe != Recipe)
            {
                MyGameCard.StartTimer(1f, new TimerAction(SetRecipe), "Setting recipe", GetActionId(nameof(SetRecipe)));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(SetRecipe)));
            }
            base.UpdateCard();
        }

        public void UpdateDescription()
        {
            if (Recipe.Length == 0)
            {
                descriptionOverride = null;
                return;
            }
            var array = Recipe.Split(',').Select(x => WorldManager.instance.GameDataLoader.GetCardFromId(x).Name).ToArray();
            Array.Sort(array);
            descriptionOverride = string.Join(", ", array) + "\n\n" + "Use a villager to clear";
        }

        public string ComputeCurrentRecipe()
        {
            var child = MyGameCard.Child;
            if (child == null) return null;
            if (child.CardData.IsBuilding || child.CardData.MyCardType == CardType.Humans || Card.IsAlive(child)) return null;
            var recipe = child.CardData.Id;
            child = child.Child;
            while (child != null)
            {
                if (child.CardData.IsBuilding || child.CardData.MyCardType == CardType.Humans || Card.IsAlive(child)) return null;
                recipe += "," + child.CardData.Id;
                child = child.Child;
            }
            return recipe;
        }

        [TimedAction(Consts.GOLEM_MOD_CRAFTER + ".set_recipe")]
        public void SetRecipe()
        {
            Recipe = ComputeCurrentRecipe();
            UpdateDescription();
        }
    }
}
