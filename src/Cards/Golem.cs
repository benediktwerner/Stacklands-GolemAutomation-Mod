﻿using System.Collections.Generic;
using UnityEngine;

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

        private bool _hasSellingModule = false;
        public bool HasSellingModule {
            get => _hasSellingModule;
            set {
                _hasSellingModule = value;
                SellingModules = value ? 1 : 0;
            }
        }

        public int CarryingCapacity = 1;
        public int ModulesLeft = 2;

        public new void Start()
        {
            _hasSellingModule = SellingModules > 0;
            UpdateDescription();
        }

        public override void UpdateDescription()
        {
            base.UpdateDescription();
            if (Counter > 0)
            {
                var s = "Counter: " + Counter;
                if (descriptionOverride == null) descriptionOverride = s;
                else descriptionOverride += "\n\n" + s;
            }
        }

        public override bool CanHaveCard(CardData otherCard)
        {
            var child = MyGameCard.Child;
            if (child == null)
            {
                if (otherCard is LocationGlyph && (otherCard.MyGameCard.Child == null || (otherCard.MyGameCard.Child.CardData is LocationGlyph && otherCard.MyGameCard.Child.Child == null))) return true;
                if (otherCard is Golem) return true;
                if (otherCard is GolemModule) return true;
                if (otherCard.MyGameCard.Child != null) return false;
                if (otherCard.MyCardType == CardType.Humans) return true;
                if (otherCard is Filter) return true;
                return false;
            }
            if (HasCardOnTop<LocationGlyph>(out var g1))
            {
                if (g1.HasCardOnTop<LocationGlyph>(out var g2))
                {
                    return g2.MyGameCard.GetChildCount() + otherCard.MyGameCard.GetChildCount() + 1 <= CarryingCapacity;
                }
                return g1.MyGameCard.GetChildCount() + otherCard.MyGameCard.GetChildCount() + 1 <= CarryingCapacity;
            }
            if (child.CardData is Golem) return otherCard is Golem;
            if (child.CardData is GolemModule) return otherCard is GolemModule;
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
            if (child == null) return false;
            if (child.MyCardType == CardType.Humans)
            {
                if (SpeedModules > 0 || HasSellingModule || Counter > 0)
                {
                    MyGameCard.StartTimer(10f, new TimerAction(RemoveModules), "Removing module", GetActionId(nameof(RemoveModules)));
                    return true;
                }
                return false;
            }
            if (child is GolemModule g)
            {
                if (ModulesLeft > 0 && g.CanInsert(this))
                {
                    MyGameCard.StartTimer(10f, new TimerAction(InsertModule), "Inserting module", GetActionId(nameof(InsertModule)));
                    return true;
                }
                return false;
            }
            if (child is LocationGlyph)
            {
                MyGameCard.StartTimer(5f * SpeedModifier, new TimerAction(Work), "Working", GetActionId(nameof(Work)));
                return true;
            }
            return false;
        }

        public static bool MoveGold(GameCard first, GameCard target, int spaceLeft)
        {
            if (first == null || spaceLeft == 0) return false;
            var parent = first.Parent;
            var currency = WorldManager.instance.CurrentBoard.Id == "main" ? "gold" : "shell";
            var gold = new List<GameCard>();
            var notGold = new List<GameCard>();
            do
            {
                if (first.CardData.Id == currency) gold.Add(first);
                else notGold.Add(first);
                first = first.Child;
            }
            while (first != null && gold.Count < spaceLeft);
            if (gold.Count == 0) return false;
            Card.Restack(gold);
            Card.Restack(notGold);
            first = gold[0];
            if (target.CardData.CanHaveCardOnTop(first.CardData))
            {
                Card.BounceTo(first, target);
                if (notGold.Count > 0) Card.Parent(parent, notGold[0]);
                else parent.Child = null;
                return true;
            }
            Card.Parent(parent, first);
            if (notGold.Count > 0) Card.Parent(gold[gold.Count - 1], notGold[0]);
            return false;
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
                if (afterEnd != null) afterEnd.Parent = parent;
                Card.BounceTo(first, target);
                return true;
            }
            moveEnd.Child = afterEnd;
            first.Parent = parent;
            return false;
        }

        public static bool PopGoldAndSell(GameCard first)
        {
            if (first == null) return false;
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
                if (first.CardData.Id == currency) gold.Add(first);
                else if (first.CardData.Value == -1) unsellable.Add(first);
                else sellable.Add(first);
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
                if (first.CardData.Value == -1) unsellable.Add(first);
                else sellable.Add(first);
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
            if (card == null) return;
            card.Parent.Child = null;
            var list = new List<GameCard>();
            do
            {
                if (list.Count == 0 || list[list.Count - 1].CardData.CanHaveCardOnTop(card.CardData))
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

        public static void PopAnimals(GameCard card)
        {
            while (card != null)
            {
                if (Card.IsAlive(card))
                {
                    var nxt = card.Child;
                    if (card.Child != null) card.Child.Parent = card.Parent;
                    card.Parent.Child = card.Child;
                    card.Parent = null;
                    card.Child = null;
                    card.SendIt();
                    card = nxt;
                }
                else card = card.Child;
            }
        }

        [TimedAction(Consts.GOLEM + ".work")]
        public void Work()
        {
            if (MyGameCard.Child?.CardData is LocationGlyph g1)
            {
                if (g1.HasCardOnTop<LocationGlyph>(out var g2))
                {
                    var card = g2.MyGameCard.Child;
                    if (card != null)
                    {
                        if (g2.target != null && g2.target.MyBoard.IsCurrent)
                        {
                            var targetRoot = g2.target.GetRootCard();
                            var spaceLeft = targetRoot.CardData is Chest ? 30 : 29 - targetRoot.GetChildCount();
                            var targetLeaf = g2.target.GetLeafCard();
                            if (HasSellingModule)
                            {
                                if (MoveGold(card, targetLeaf, spaceLeft))
                                {
                                    PopAnimals(card);
                                    return;
                                }
                            }
                            else if (MoveCards(card, targetLeaf, spaceLeft))
                            {
                                PopAnimals(card);
                                return;
                            }
                        }
                        PopAnimals(card);
                        if (HasSellingModule)
                        {
                            if (Sell(card)) return;
                        }
                    }
                }
                else if (HasSellingModule)
                {
                    if (PopGoldAndSell(g1.MyGameCard.Child)) return;
                }
                else
                {
                    PopAll(g1.MyGameCard.Child);
                }

                if (g1.target != null && g1.target.MyBoard.IsCurrent && g1.target.Child != null)
                {
                    var spaceLeft = CarryingCapacity;
                    var jumpTarget = g2 == null ? g1.MyGameCard : g2.MyGameCard;
                    if (Counter > 0)
                    {
                        var take = g1.target.GetChildCount() - Counter;
                        if (take <= 0) return;
                        if (take < spaceLeft) spaceLeft = take;
                    }
                    while (jumpTarget.Child != null)
                    {
                        jumpTarget = jumpTarget.Child;
                        spaceLeft--;
                    }
                    if (spaceLeft <= 0) return;
                    var card = g1.target.GetLeafCard();
                    var move = new List<GameCard>();
                    var leftover = new List<GameCard>();
                    while (card != g1.target && spaceLeft-- > 0)
                    {
                        if ((filter.Count == 0 || filter.Contains(card.CardData.Id)) && (!HasSellingModule || card.CardData.Value != -1))
                        {
                            move.Add(card);
                        }
                        else leftover.Add(card);
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
                        else card.Child = null;
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
                AudioManager.me.PlaySound2D(AudioManager.me.CardDestroy, Random.Range(0.8f, 1.2f), 0.3f);
                UpdateDescription();
            }
        }

        [TimedAction(Consts.GOLEM + ".remove_modules")]
        public void RemoveModules()
        {
            var removed = new List<GameCard>();
            if (HasSellingModule)
            {
                removed.Add(WorldManager.instance.CreateCard(transform.position, Consts.GOLEM_MOD_SELL, checkAddToStack: false).MyGameCard);
            }
            for (var i = 0; i < SpeedModules; i++)
            {
                removed.Add(WorldManager.instance.CreateCard(transform.position, Consts.GOLEM_MOD_SPEED, checkAddToStack: false).MyGameCard);
            }
            if (Counter > 0)
            {
                var card = (GolemModuleCounter) WorldManager.instance.CreateCard(transform.position, Consts.GOLEM_MOD_COUNTER, checkAddToStack: false);
                card.Counter = Counter;
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
            UpdateDescription();
        }
    }
}