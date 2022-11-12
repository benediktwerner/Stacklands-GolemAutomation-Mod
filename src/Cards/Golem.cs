using System;
using System.Collections.Generic;
using System.Linq;

namespace GolemAutomation
{
    class Golem : HasFilter
    {
        public override bool DetermineCanHaveCardsWhenIsRoot => true;

        public override bool CanHaveCardsWhileHasStatus() => true;

        [ExtraData(Consts.GOLEM + ".speed_modules")]
        public int SpeedModules = 0;

        [ExtraData(Consts.GOLEM + ".speed_modifier")]
        public float SpeedModifier = 1f;
        public float BaseSpeedModifier = 1f;

        [ExtraData(Consts.GOLEM + ".counter")]
        public int Counter = 0;

        [ExtraData(Consts.GOLEM + ".selling_module")]
        public int SellingModules = 0;
        public bool HasSellingModule
        {
            get => SellingModules > 0;
            set => SellingModules = value ? 1 : 0;
        }

        [ExtraData(Consts.GOLEM + ".crafting_recipe")]
        public string CraftingRecipe = "";
        public bool HasRecipe => CraftingRecipe.Length > 0;

        [ExtraData(Consts.GOLEM + ".modules_left")]
        public int ModulesLeft = 2;

        public int CarryingCapacity = 1;
        public string ModulePostfix = "";

        public bool HasModule => Counter > 0 || HasSellingModule || HasRecipe || SpeedModules > 0;

        public new void Start()
        {
            UpdateModulePostfix();
            UpdateDescription();
            base.Start();
        }

        public void UpdateModulePostfix()
        {
            var modules = new List<string>();
            if (SpeedModules > 0)
                modules.Add(SpeedModules.ToString());
            if (HasSellingModule)
                modules.Add("$");
            if (HasRecipe)
                modules.Add("C");
            if (Counter > 0)
                modules.Add("#");
            if (modules.Count > 0)
                ModulePostfix = " +" + string.Join("/", modules);
            else
                ModulePostfix = "";
        }

        public override void UpdateDescription()
        {
            if (HasRecipe)
            {
                var array = CraftingRecipe
                    .Split(',')
                    .Select(x => WorldManager.instance.GameDataLoader.GetCardFromId(x).Name)
                    .ToArray();
                Array.Sort(array);
                descriptionOverride = "Recipe: " + string.Join(", ", array);
            }
            else
            {
                base.UpdateDescription();
                if (!HasModule)
                    return;
            }
            if (descriptionOverride == null)
            {
                descriptionOverride = Description;
            }
            descriptionOverride += "\n\n";
            if (Counter > 0)
            {
                descriptionOverride += "Counter: " + Counter + "\n";
            }
            descriptionOverride +=
                "Remaining Module Slots: " + ModulesLeft + "\nUse a villager to remove modules";
        }

        public override bool CanHaveCard(CardData otherCard)
        {
            var child = MyGameCard.Child;
            if (child == null)
            {
                if (
                    otherCard is Glyph
                    && (
                        otherCard.MyGameCard.Child == null
                        || (
                            otherCard.MyGameCard.Child.CardData is LocationGlyph
                            && otherCard.MyGameCard.Child.Child == null
                        )
                    )
                )
                    return true;
                if (otherCard is Golem)
                    return true;
                if (otherCard is GolemModule)
                    return true;
                if (otherCard.MyGameCard.Child != null)
                    return false;
                if (otherCard.MyCardType == CardType.Humans)
                    return true;
                if (otherCard is Filter)
                    return true;
                return false;
            }
            if (HasCardOnTop<Glyph>(out var g1))
            {
                if (g1.HasCardOnTop<LocationGlyph>(out var g2))
                {
                    return g2.MyGameCard.GetChildCount() + otherCard.MyGameCard.GetChildCount() + 1
                            <= CarryingCapacity
                        || Card.IsCurrencyStack(otherCard);
                }
                return g1.MyGameCard.GetChildCount() + otherCard.MyGameCard.GetChildCount() + 1
                        <= CarryingCapacity
                    || Card.IsCurrencyStack(otherCard);
            }
            if (child.CardData is Golem)
                return otherCard is Golem;
            if (child.CardData is GolemModule)
                return otherCard is GolemModule;
            return false;
        }

        public override void UpdateCard()
        {
            if (!TryStartAction() && MyGameCard.TimerRunning)
            {
                MyGameCard.CancelTimer(GetActionId(nameof(Work)));
                MyGameCard.CancelTimer(GetActionId(nameof(RemoveModules)));
                MyGameCard.CancelTimer(GetActionId(nameof(InsertModule)));
            }
            base.UpdateCard();
        }

        public bool TryStartAction()
        {
            var child = MyGameCard.Child?.CardData;
            if (child == null)
                return false;
            if (child.MyCardType == CardType.Humans)
            {
                if (HasModule)
                {
                    MyGameCard.StartTimer(
                        10f,
                        new TimerAction(RemoveModules),
                        "Removing module",
                        GetActionId(nameof(RemoveModules))
                    );
                    return true;
                }
                return false;
            }
            if (child is GolemModule g)
            {
                if (ModulesLeft > 0 && g.CanInsert(this))
                {
                    MyGameCard.StartTimer(
                        10f,
                        new TimerAction(InsertModule),
                        "Inserting module",
                        GetActionId(nameof(InsertModule))
                    );
                    return true;
                }
                return false;
            }
            if (child is Glyph)
            {
                MyGameCard.StartTimer(
                    5f * SpeedModifier,
                    new TimerAction(Work),
                    "Working",
                    GetActionId(nameof(Work))
                );
                return true;
            }
            return false;
        }

        public static bool MoveGold(GameCard first, GameCard target, int spaceLeft)
        {
            if (first == null || spaceLeft == 0)
                return false;
            var parent = first.Parent;
            var currency = Card.Currency;
            var gold = new List<GameCard>();
            var notGold = new List<GameCard>();
            do
            {
                if (first.CardData.Id == currency)
                    gold.Add(first);
                else
                    notGold.Add(first);
                first = first.Child;
            } while (first != null && gold.Count < spaceLeft);
            while (first != null)
            {
                notGold.Add(first);
                first = first.Child;
            }
            if (gold.Count == 0)
                return false;
            Card.Restack(gold);
            Card.Restack(notGold);
            first = gold[0];
            var targetRoot = target.GetRootCard();
            if (targetRoot.CardData is Chest chest)
            {
                var goldCount = gold.Count;
                while (goldCount > 0)
                {
                    var chestWithSpace = chest.GetChestWithSpace();
                    if (chestWithSpace == null)
                    {
                        break;
                    }
                    var add = Math.Min(
                        chestWithSpace.maxCoinCount - chestWithSpace.CoinCount,
                        goldCount
                    );
                    chestWithSpace.CoinCount += add;
                    goldCount -= add;
                }
                foreach (var g in gold)
                    g.DestroyCard(true, true);
            }
            else if (target.CardData.CanHaveCardOnTop(first.CardData))
                Card.BounceTo(first, target);
            else
            {
                Card.Parent(parent, first);
                if (notGold.Count > 0)
                    Card.Parent(gold[gold.Count - 1], notGold[0]);
                return false;
            }

            if (notGold.Count > 0)
                Card.Parent(parent, notGold[0]);
            else
                parent.Child = null;
            return true;
        }

        public static bool MoveCards(GameCard first, GameCard target, int spaceLeft)
        {
            var moveEnd = first;
            for (; spaceLeft > 1 && moveEnd.Child != null; spaceLeft--)
            {
                moveEnd = moveEnd.Child;
            }
            var parent = first.Parent;
            var afterEnd = moveEnd.Child;
            first.Parent = null;
            moveEnd.Child = null;
            if (target.CardData.CanHaveCardOnTop(first.CardData))
            {
                parent.Child = afterEnd;
                if (afterEnd != null)
                    afterEnd.Parent = parent;
                Card.BounceTo(first, target);
                return true;
            }
            moveEnd.Child = afterEnd;
            first.Parent = parent;
            return false;
        }

        public static bool PopGoldAndSell(GameCard first)
        {
            if (first == null)
                return false;
            var parent = first.Parent;
            parent.Child = null;
            var currency = Card.Currency;
            var gold = new List<GameCard>();
            var sellable = new List<GameCard>();
            var unsellable = new List<GameCard>();
            do
            {
                if (Card.IsAlive(first))
                {
                    var nxt = first.Child;
                    first.Parent = null;
                    first.Child = null;
                    first.SendIt();
                    first = nxt;
                    continue;
                }
                if (first.CardData.Id == currency)
                    gold.Add(first);
                else if (first.CardData.Value == -1)
                    unsellable.Add(first);
                else
                    sellable.Add(first);
                first = first.Child;
            } while (first != null);
            if (gold.Count > 0)
            {
                Card.Restack(gold);
                WorldManager.instance.StackSend(gold[0]);
            }
            if (unsellable.Count > 0)
            {
                Card.Restack(unsellable);
                Card.Parent(parent, unsellable[0]);
            }
            if (sellable.Count > 0)
            {
                var coins = Card.Sell(sellable, parent.transform.position);
                if (coins != null)
                {
                    Card.InsertBelow(parent, coins);
                }
                return true;
            }
            return false;
        }

        public static bool Sell(GameCard first)
        {
            var parent = first.Parent;
            parent.Child = null;
            var sellable = new List<GameCard>();
            var unsellable = new List<GameCard>();
            do
            {
                if (first.CardData.Value == -1)
                    unsellable.Add(first);
                else
                    sellable.Add(first);
                first = first.Child;
            } while (first != null);
            if (unsellable.Count > 0)
            {
                Card.Restack(unsellable);
                Card.Parent(parent, unsellable[0]);
            }
            if (sellable.Count > 0)
            {
                var coins = Card.Sell(sellable, parent.transform.position);
                if (coins != null)
                {
                    Card.InsertBelow(parent, coins);
                }
                return true;
            }
            return false;
        }

        public static void PopAll(GameCard card)
        {
            if (card == null)
                return;
            card.Parent.Child = null;
            var list = new List<GameCard>();
            do
            {
                if (
                    list.Count == 0 || list[list.Count - 1].CardData.CanHaveCardOnTop(card.CardData)
                )
                {
                    list.Add(card);
                }
                else
                {
                    Card.Restack(list);
                    WorldManager.instance.StackSend(list[0]);
                    list = new List<GameCard> { card };
                }
                card = card.Child;
            } while (card != null);
            if (list.Count > 0)
            {
                Card.Restack(list);
                WorldManager.instance.StackSend(list[0]);
            }
        }

        public bool Craft(GameCard card, GameCard target)
        {
            var parent = card.Parent;
            var leftover = new List<GameCard>();
            var craft = new List<GameCard>();
            var recipe = CraftingRecipeDict();
            var recipeCount = recipe.Values.Sum();
            do
            {
                if (recipe.TryGetValue(card.CardData.Id, out int c))
                {
                    if (c > 0)
                    {
                        recipe[card.CardData.Id]--;
                        craft.Add(card);
                    }
                    else
                        leftover.Add(card);
                    card = card.Child;
                }
                else
                {
                    card = Card.PopAndGetChild(card);
                }
            } while (card != null);
            if (
                craft.Count == recipeCount
                && (target == null || target.GetRootCard().GetChildCount() + craft.Count < 30)
            )
            {
                Card.Restack(craft);
                if (target == null)
                    craft[0].SendIt();
                else
                    Card.BounceTo(craft[0], target.GetLeafCard());
                Card.Restack(leftover);
                if (leftover.Count > 0)
                    Card.Parent(parent, leftover[0]);
                else
                    parent.Child = null;
                return true;
            }
            return false;
        }

        public Dictionary<string, int> CraftingRecipeDict()
        {
            var recipe = new Dictionary<string, int>();
            foreach (var id in CraftingRecipe.Split(','))
            {
                recipe.TryGetValue(id, out int c);
                recipe[id] = c + 1;
            }
            return recipe;
        }

        public static void PopAnimals(GameCard card)
        {
            while (card != null)
            {
                if (Card.IsAlive(card))
                    card = Card.PopAndGetChild(card);
                else
                    card = card.Child;
            }
        }

        [TimedAction(Consts.GOLEM + ".work")]
        public void Work()
        {
            GameCard target = null;
            GameCard parent = MyGameCard;
            if (MyGameCard.Child?.CardData is Glyph g1)
            {
                if (g1.HasCardOnTop<LocationGlyph>(out var g2))
                {
                    parent = g2.MyGameCard;
                    target = g2.target;
                }
                else
                    parent = g1.MyGameCard;

                if (target != null && !target.MyBoard.IsCurrent)
                    target = null;

                var card = parent.Child;
                if (card != null)
                {
                    if (HasRecipe)
                    {
                        if (Craft(card, target))
                            return;
                    }
                    else if (target != null)
                    {
                        var targetRoot = target.GetRootCard();
                        var spaceLeft =
                            targetRoot.CardData is Chest ? 30 : 29 - targetRoot.GetChildCount();
                        var targetLeaf = target.GetLeafCard();
                        if (HasSellingModule)
                        {
                            if (MoveGold(card, targetLeaf, spaceLeft))
                            {
                                PopAnimals(parent);
                                return;
                            }
                            if (Sell(card))
                            {
                                PopAnimals(parent);
                                return;
                            }
                        }
                        else if (MoveCards(card, targetLeaf, spaceLeft))
                        {
                            PopAnimals(parent);
                            return;
                        }
                        PopAnimals(card);
                    }
                    else if (HasSellingModule)
                    {
                        PopAnimals(card);
                        if (PopGoldAndSell(card))
                            return;
                    }
                    else
                    {
                        PopAll(card);
                    }
                }

                var spaceLeftBase = CarryingCapacity;
                var jumpTarget = parent;
                var recipeDictBase = HasRecipe ? CraftingRecipeDict() : null;
                while (jumpTarget.Child != null)
                {
                    jumpTarget = jumpTarget.Child;
                    spaceLeftBase--;
                    if (
                        recipeDictBase != null
                        && recipeDictBase.TryGetValue(jumpTarget.CardData.Id, out int c)
                        && c > 0
                    )
                    {
                        recipeDictBase[jumpTarget.CardData.Id]--;
                    }
                }

                foreach (var source in g1.FindTargets())
                {
                    var recipeDict =
                        recipeDictBase == null ? null : new Dictionary<string, int>(recipeDictBase);
                    var spaceLeft = spaceLeftBase;
                    if (Counter > 0)
                    {
                        var take = source.GetChildCount() - Counter;
                        if (take <= 0)
                            continue;
                        if (take < spaceLeft)
                            spaceLeft = take;
                    }
                    if (spaceLeft <= 0)
                        continue;
                    card = source.GetLeafCard();
                    var move = new List<GameCard>();
                    var leftover = new List<GameCard>();
                    while (card != source && spaceLeft > 0)
                    {
                        if (
                            (
                                recipeDict == null
                                || recipeDict.TryGetValue(card.CardData.Id, out var c) && c > 0
                            )
                            && (
                                (filter.Count == 0 || filter.Contains(card.CardData.Id))
                                && (!HasSellingModule || card.CardData.Value != -1)
                            )
                        )
                        {
                            if (recipeDict != null)
                                recipeDict[card.CardData.Id]--;
                            spaceLeft--;
                            move.Add(card);
                        }
                        else
                            leftover.Add(card);
                        card = card.Parent;
                    }
                    if (move.Count > 0)
                    {
                        Card.Restack(move);
                        Card.BounceTo(move[0], jumpTarget);
                        if (leftover.Count > 0)
                        {
                            Card.Restack(leftover);
                            Card.Parent(card, leftover[0]);
                        }
                        else
                            card.Child = null;
                        return;
                    }
                }
            }
        }

        [TimedAction(Consts.GOLEM + ".insert_module")]
        public void InsertModule()
        {
            if (MyGameCard.Child?.CardData is GolemModule g)
            {
                ModulesLeft--;
                g.Insert(this);
                DestroyChildrenMatchingPredicateAndRestack(c => g, 1);
                AudioManager.me.PlaySound2D(
                    AudioManager.me.CardDestroy,
                    UnityEngine.Random.Range(0.8f, 1.2f),
                    0.3f
                );
                UpdateModulePostfix();
                UpdateDescription();
            }
        }

        [TimedAction(Consts.GOLEM + ".remove_modules")]
        public void RemoveModules()
        {
            var removed = new List<GameCard>();
            if (HasSellingModule)
            {
                removed.Add(
                    WorldManager.instance
                        .CreateCard(
                            transform.position,
                            Consts.GOLEM_MOD_SELL,
                            checkAddToStack: false
                        )
                        .MyGameCard
                );
            }
            for (var i = 0; i < SpeedModules; i++)
            {
                removed.Add(
                    WorldManager.instance
                        .CreateCard(
                            transform.position,
                            Consts.GOLEM_MOD_SPEED,
                            checkAddToStack: false
                        )
                        .MyGameCard
                );
            }
            if (Counter > 0)
            {
                var card = (GolemModuleCounter)
                    WorldManager.instance.CreateCard(
                        transform.position,
                        Consts.GOLEM_MOD_COUNTER,
                        checkAddToStack: false
                    );
                card.Counter = Counter;
                card.UpdateDescription();
                removed.Add(card.MyGameCard);
            }
            if (HasRecipe)
            {
                var card = (GolemModuleCrafter)
                    WorldManager.instance.CreateCard(
                        transform.position,
                        Consts.GOLEM_MOD_CRAFTER,
                        checkAddToStack: false
                    );
                card.Recipe = CraftingRecipe;
                card.UpdateDescription();
                removed.Add(card.MyGameCard);
            }
            if (removed.Count > 0)
            {
                ModulesLeft += removed.Count;
                Card.Restack(removed);
                WorldManager.instance.StackSend(removed[0]);
            }
            HasSellingModule = false;
            SpeedModules = 0;
            SpeedModifier = BaseSpeedModifier;
            Counter = 0;
            CraftingRecipe = "";
            UpdateModulePostfix();
            UpdateDescription();
        }
    }
}
